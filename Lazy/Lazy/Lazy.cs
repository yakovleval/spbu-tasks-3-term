namespace Lazy;

/// <summary>
/// class that implements ILazy interface without thread safety
/// </summary>
/// <typeparam name="T">type of returned value</typeparam>
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

    /// <summary>
    /// computes the value if it wasn't computed before and stores it
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
