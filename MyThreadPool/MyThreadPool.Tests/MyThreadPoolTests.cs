using System.Net.Http.Headers;
using System.Threading;

namespace MyThreadPool.Tests;

public class Tests
{
    private static readonly int THREADS_NUMBER = 4;

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void TestSimpleTask()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task = threadPool.Submit(() => 2 * 2);
        int result = task.Result;
        Assert.That(task.Result, Is.EqualTo(4));
    }

    [Test]
    public void TestException()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task = threadPool.Submit<int>(() => throw new Exception());
        Assert.Throws<AggregateException>(() =>
        {
            int result = task.Result;
        });
    }

    [Test]
    public void TestContinueWith()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        var task = threadPool.Submit(() => 2 * 2)
            .ContinueWith(x => x * x)
            .ContinueWith(x => x - 2);
        Assert.That(task.Result, Is.EqualTo(14));
    }

    [Test]
    public void TestSubmitThrowsExceptionAfterShutdown()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        threadPool.ShutDown();
        Assert.Throws<ThreadPoolShutDownException>(() => threadPool.Submit(() => 2 * 2));
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
    public void TestWorkingThreadsNumber()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        ManualResetEvent submitEvent = new(false);
        for (int i = 0; i < THREADS_NUMBER; i++)
        {
            new Thread(() =>
            {
                submitEvent.WaitOne();
                threadPool.Submit(() =>
                {
                    Thread.Sleep(3000);
                    return 0;
                });
            }).Start();
        }
        submitEvent.Set();
        Thread.Sleep(500);
        Assert.That(threadPool.WorkingThreadsNumber, Is.EqualTo(THREADS_NUMBER));
        threadPool.ShutDown();
        Assert.That(threadPool.WorkingThreadsNumber, Is.EqualTo(0));
    }

    [Test]
    public void TestNoThreadsAreRunning()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        Assert.That(threadPool.WorkingThreadsNumber, Is.EqualTo(0));
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
}