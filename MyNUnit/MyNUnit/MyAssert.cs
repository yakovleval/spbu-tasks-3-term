namespace MyNUnit;

public class MyAssertionException : System.Exception
{
    public MyAssertionException(string message = "") : base(message) { }
}

public static class MyAssert
{
    public static void IsTrue(bool predicate)
    {
        if (!predicate)
        {
            throw new MyAssertionException("expected: true, but was: false");
        }
    }

    //public static void Throws<TException>(Action action) where TException : Exception
    //{
    //    Exception? caught = null;
    //    try
    //    {
    //        action();
    //    }
    //    catch(Exception e)
    //    {
    //        caught = e;
    //    }
    //    if (caught is null)
    //    {
    //        throw new MyAssertionException($"expected: {typeof(TException)}, but no exception was thrown");
    //    }
    //    throw new MyAssertionException($"expected: {typeof(TException)}, but was: {caught.GetType()}");
    //}
}
