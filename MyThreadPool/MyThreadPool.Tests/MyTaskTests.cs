namespace MyThreadPool.Tests;

public class MyTaskTests
{
    private static readonly int THREADS_NUMBER = 4;

    [Test]
    public void TestContinueWith()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task = threadPool.Submit(() => 2 * 2)
            .ContinueWith(x => x * x)
            .ContinueWith(x => x - 2)
            .ContinueWith(x => x * 2);
        Assert.That(task.Result, Is.EqualTo(28));
    }

    [Test]
    public void TestContinueWithThrowsExceptionAfterShutdown()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task1 = threadPool.Submit(() => 2 * 2);
        threadPool.ShutDown();
        Assert.Throws<ThreadPoolShutDownException>(() => task1.ContinueWith(x => x * x));
    }

    [Test]
    public void TestExceptionBeforeContinueWith()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task = threadPool.Submit<int>(() => throw new Exception())
            .ContinueWith(x => x * x)
            .ContinueWith(x => x - 2);
        Assert.Throws<AggregateException>(() => _ = task.Result);
    }

    [Test]
    public void TestContinueWithTasksRunAfterShutdown()
    {
        MyThreadPool threadPool = new(1);
        int sum = 0;
        var task = threadPool.Submit(() =>
        {
            Thread.Sleep(2000);
            Interlocked.Increment(ref sum);
            return 0;
        });
        task.ContinueWith((x) =>
        {
            Thread.Sleep(2000);
            Interlocked.Increment(ref sum);
            return 0;
        });
        threadPool.ShutDown();
        Assert.That(sum, Is.EqualTo(2));
    }
}
