using System.Net;

namespace Tests
{
    public class Tests
    {
        private Server.Server server;
        private Client.Client client;
        private static readonly string IP = "127.0.0.1";
        private static readonly int PORT = 8888;

        [SetUp]
        public void SetUp()
        {
            server = new(IPAddress.Parse(IP), PORT);
            Task.Run(() => server.StartAsync());
            client = new(IP, PORT);
        }

        [TearDown]
        public void TearDown()
        {
            //server.Stop();
            client.Dispose();
        }

        [Test]
        public async Task TestListExistingFolder()
        {
            var result = await client.ListAsync("../../../TestFolder/");
            var expected = "3 ..\\..\\..\\TestFolder\\folder1 true ..\\..\\..\\TestFolder\\1.txt false ..\\..\\..\\TestFolder\\2.txt false";
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
            var result = await client.GetAsync("..\\..\\..\\TestFolder\\1.txt");
            var expected = File.ReadAllBytes("..\\..\\..\\TestFolder\\1.txt");
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
            for (int i = 0; i < numberOfClients; i++)
            {
                clients[i] = new Client.Client(IP, PORT);
            }
            List<byte[]> results = new();
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
            }
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