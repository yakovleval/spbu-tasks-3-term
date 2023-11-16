namespace Lazy;

public class LazyThreadSafe<T> : ILazy<T>
{
    Func<T?> supplier;
    T? result;
    volatile Exception? exception;
    volatile bool isEvaluated = false;
    private readonly object locker = new();
    public LazyThreadSafe(Func<T?> supplier)
    {
        ArgumentNullException.ThrowIfNull(supplier);
        this.supplier = supplier;
    }

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
            isEvaluated = true;
        try
        {
            result = supplier();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        return result;
    }
        }
}
