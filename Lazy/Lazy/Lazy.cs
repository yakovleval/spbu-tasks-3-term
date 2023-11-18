namespace Lazy;

public class Lazy<T> : ILazy<T>
{
    private readonly Func<T?>? supplier;
    private T? result;
    private Exception? exception;
    private bool isEvaluated = false;

    public Lazy(Func<T?>? supplier)
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
        isEvaluated = true;
        try
        {
            result = supplier!();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        return result;
    }
}
