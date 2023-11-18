namespace Lazy;

public class LazyThreadSafe<T> : ILazy<T>
{
    private readonly Func<T?>? supplier;
    private T? result;
    private volatile Exception? exception;
    private volatile bool isEvaluated = false;
    private readonly object locker = new();

    public LazyThreadSafe(Func<T?> supplier)
    {
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
        //lock (locker)
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
