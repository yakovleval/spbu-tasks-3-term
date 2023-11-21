using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool;

public class MyThreadPool
{
    private ConcurrentQueue<Action> tasksQueue = new();
    private AutoResetEvent queueIsNotEmptyEvent = new(false);
    private ManualResetEvent shutDownEvent = new(false);
    private WaitHandle[] events;
    private Thread[] threads;
    private CancellationTokenSource source = new();
    private volatile int workingThreadsNumber = 0;

    public MyThreadPool(int threadsNumber)
    {
        events = new WaitHandle[]
        {
            queueIsNotEmptyEvent,
            shutDownEvent
        };
        threads = new Thread[threadsNumber];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() =>
            {
                while (!source.IsCancellationRequested)
                {
                    int eventIndex = WaitHandle.WaitAny(events);
                    if (eventIndex == 1)
                        break;
                    queueIsNotEmptyEvent.Reset()
                    if (!tasksQueue.TryDequeue(out Action? task))
                        continue;
                    else if (task != null)
                    {
                        workingThreadsNumber++;
                        task();
                        workingThreadsNumber--;
                    }
                }
                while (tasksQueue.Count > 0)
                {
                    if (!tasksQueue.TryDequeue(out Action? task))
                        break;
                    else if (task != null)
                    {
                        workingThreadsNumber++;
                        task();
                        workingThreadsNumber--;
                    }
                }
            });
            threads[i].Start();
        }
    }
}