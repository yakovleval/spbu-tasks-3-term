using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> _tasksQueue = new();
    private readonly AutoResetEvent _queueIsNotEmptyEvent = new(false);
    private readonly ManualResetEvent _shutDownEvent = new(false);
    private readonly  WaitHandle[] _events;
    private readonly Thread[] _threads;
    private readonly CancellationTokenSource _source = new();
    private volatile int _workingThreadsNumber = 0;

    public MyThreadPool(int threadsNumber)
    {
        _events = new WaitHandle[]
        {
            _queueIsNotEmptyEvent,
            _shutDownEvent
        };
        _threads = new Thread[threadsNumber];
        for (int i = 0; i < _threads.Length; i++)
        {
            _threads[i] = new Thread(() =>
            {
                WaitHandle.WaitAny(_events);
                while (!_source.IsCancellationRequested)
                {
                    if (_tasksQueue.TryDequeue(out Action? task) && task != null)
                    {
                        _workingThreadsNumber++;
                        task();
                        _workingThreadsNumber--;
                    }
                    lock (_tasksQueue)
                    {
                        if (_tasksQueue.IsEmpty)
                            _queueIsNotEmptyEvent.Reset();
                    }
                    WaitHandle.WaitAny(_events);
                }
                while (!_tasksQueue.IsEmpty)
                {
                    if (!_tasksQueue.TryDequeue(out Action? task))
                        break;
                    else if (task != null)
                    {
                        _workingThreadsNumber++;
                        task();
                        _workingThreadsNumber--;
                    }
                }
            });
            _threads[i].Start();
        }
    }

    public void ShutDown()
    {
        lock (_source)
            _source.Cancel();
        _shutDownEvent.Set();
        for (int i = 0; i < _threads.Length; i++)
            _threads[i].Join();
    }

    public IMyTask<TResult> Submit<TResult>(Func<TResult> supplier)
    {
        lock (_source)
        {
            if (_source.IsCancellationRequested)
                throw new InvalidOperationException("thread was shut down");
            var task = new MyTask<TResult>(this, supplier);
            _tasksQueue.Enqueue(() => task.Run());
            return task;
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private TResult? _result;
        private readonly Func<TResult> _supplier;
        private Exception? _exception;
        Action? _nextAction;
        private readonly MyThreadPool _threadPool;
        private readonly ManualResetEvent _resultEvent = new(false);
        public bool IsCompleted { get; private set; }

        public TResult? Result
        {
            get
            {
                _resultEvent.WaitOne();
                if (_exception != null)
                    throw new AggregateException(_exception);
                return _result;
            }
        }

        public MyTask(MyThreadPool threadPool, Func<TResult> supplier)
        {
            _supplier = supplier;
            _threadPool = threadPool;
        }

        public void Run()
        {
            try
            {
                _result = _supplier();
            }

            catch (Exception e)
            {
                _exception = e;
            }
            finally
            {
                IsCompleted = true;
                if (_nextAction != null)
                {
                    _threadPool._tasksQueue.Enqueue(_nextAction);
                }
                _resultEvent.Set();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuation)
        {
            lock (_threadPool._source)
            {
                if (_threadPool._source.IsCancellationRequested)
                {
                    throw new InvalidOperationException("thread was shut down");
                }
                var task = new MyTask<TNewResult>(_threadPool, () => continuation(Result));
                _nextAction = () => task.Run();
                return task;
            }
        }
    }
}