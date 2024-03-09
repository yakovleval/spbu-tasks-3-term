namespace MyThreadPool;

/// <summary>
/// represents a task to be run in a thread pool with return value of type TResult
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// returns true if task is completed, false otherwise
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// returns result of a task
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// submits new task to a thread pool using the result of this task
    /// </summary>
    /// <typeparam name="TNewResult">type of return value of new task</typeparam>
    /// <param name="continuation">new function to run</param>
    /// <returns>new task with given function</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> continuation);
}
