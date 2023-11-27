using System.Collections.Concurrent;
using System.Net;

namespace Tests
{
    public class Tests
    {
        private Server.Server server;
        private Client.Client client;
        private static readonly int PORT = 25565;

        [OneTimeSetUp]
        public void SetUp()
        {
            server = new(IPAddress.Loopback, PORT);
            Task.Run(() => server.StartAsync());
            Thread.Sleep(1000);
            client = new(IPAddress.Loopback, PORT);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            server.Stop();
            client.Dispose();
        }

        [Test]
        public async Task TestListExistingFolder()
        {
            var result = await client.ListAsync("../../../TestFolder");
            var expected = "3 ../../../TestFolder/folder1 true ../../../TestFolder/1.txt false ../../../TestFolder/2.txt false";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task TestListNonExistingFolder()
        {
            var result = await client.ListAsync("../../../NonExistingFolder/");
            var expected = "directory not found";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task TestGetExistingFile()
        {
            var result = await client.GetAsync("../../../TestFolder/1.txt");
            var expected = File.ReadAllBytes("../../../TestFolder/1.txt");
            Assert.That(result.SequenceEqual(expected), Is.True);
        }

        [Test]
        public void TestGetNonExistingFile()
        {
            object value = Assert.ThrowsAsync<FileNotFoundException>(async () => await client.GetAsync("ololo"));
        }

        [Test]
        public void TestGetRequestRace()
        {
            var raceEvent = new ManualResetEvent(false);
            int numberOfClients = 8;
            string path = "../../../TestFolder/1.txt";
            var clients = new Client.Client[numberOfClients];
            int count = 0;
            for (int i = 0; i < numberOfClients; i++)
            {
                clients[i] = new Client.Client(IPAddress.Loopback, PORT);
                count++;
            }
            Console.WriteLine($"clients: {count}");
            count = 0;
            ConcurrentBag<byte[]> results = new();
            var threads = new Thread[numberOfClients];
            for (int i = 0; i < numberOfClients; i++)
            {
                int localI = i;
                threads[i] = new Thread(() =>
                {
                    raceEvent.WaitOne();
                    results.Add(clients[localI].GetAsync(path).Result);
                });
                threads[i].Start();
                count++;
            }
            Console.WriteLine($"threads started: {count}");
            raceEvent.Set();
            foreach (var thread in threads)
            {
                thread.Join();
            }
            var expected = File.ReadAllBytes(path);

            Assert.That(results.Count, Is.EqualTo(numberOfClients));
            foreach (var result in results)
            {
                Assert.That(result.SequenceEqual(expected), Is.True);
            }
        }
    }
}