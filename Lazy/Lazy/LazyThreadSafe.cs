namespace Lazy;

/// <summary>
/// class that implements ILazy interface with thread safety
/// </summary>
/// <typeparam name="T">type of retuned value</typeparam>
public class LazyThreadSafe<T> : ILazy<T>
{
    private readonly Func<T?>? supplier;
    private T? result;
    private volatile Exception? exception;
    private volatile bool isEvaluated = false;
    private readonly object locker = new();

    public LazyThreadSafe(Func<T?>? supplier)
    {
        this.supplier = supplier;
    }

    /// <summary>
    /// computes the value (thread-safely) if it wasn't computed before and stores it
    /// </summary>
    /// <returns>computed value of type T</returns>
    public T? Get()
    {
        if (isEvaluated)
        {
            if (exception is not null)
                throw exception;
            return result;
        }
        lock (locker)
        {
            if (isEvaluated)
            {
                if (exception is not null)
                    throw exception;
                return result;
            }
            try
            {
                result = supplier!();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                isEvaluated = true;
            }
            return result;
        }
     }
}
