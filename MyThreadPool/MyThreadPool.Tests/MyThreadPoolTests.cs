using System.ComponentModel;
using System.Net.Http.Headers;
using System.Threading;

namespace MyThreadPool.Tests;

public class Tests
{
    private static readonly int THREADS_NUMBER = 4;

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
    public void TestSubmitThrowsExceptionAfterShutdown()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        threadPool.ShutDown();
        Assert.Throws<ThreadPoolShutDownException>(() => threadPool.Submit(() => 2 * 2));
    }

    [Test]
    public void TestWorkingThreadsNumber()
    {
        MyThreadPool threadPool = new(THREADS_NUMBER);
        ManualResetEvent workingEvent = new(false);
        ManualResetEvent submitEvent = new(false);
        for (int i = 0; i < THREADS_NUMBER; i++)
        {
            new Thread(() =>
            {
                submitEvent.WaitOne();
                threadPool.Submit(() =>
                {
                    workingEvent.WaitOne();
                    return 0;
                });
            }).Start();
        }
        submitEvent.Set();
        Thread.Sleep(1000);
        Assert.That(threadPool.WorkingThreadsNumber, Is.EqualTo(THREADS_NUMBER));
        workingEvent.Set();
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
    public void TestSubmittedTasksRunAfterShutdown()
    {
        MyThreadPool threadPool = new(1);
        int sum = 0;
        threadPool.Submit(() =>
        {
            Thread.Sleep(2000);
            Interlocked.Increment(ref sum);
            return 0;
        });
        threadPool.Submit(() =>
        {
            Thread.Sleep(2000);
            Interlocked.Increment(ref sum);
            return 0;
        });
        threadPool.ShutDown();
        Assert.That(sum, Is.EqualTo(2));
    }
}