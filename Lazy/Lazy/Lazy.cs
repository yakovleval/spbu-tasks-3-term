namespace Lazy;

public class Lazy<T> : ILazy<T>
{
    Func<T?> supplier;
    T? result;
    Exception? exception;
    bool isEvaluated = false;
    public Lazy(Func<T?> supplier)
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
