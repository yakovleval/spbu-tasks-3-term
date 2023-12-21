namespace Test3.Tests;

public class DummyClass
{
    private static int _privateStaticField;
    private int _privateNonStaticField;
    public static int _publicStaticField;
    public int _publicNonStaticField;
    protected static int _protectedStaticField;
    protected List<List<List<string>>> _protectedNonStaticField;

    private static List<List<List<int>>> PrivateStaticMethod(int p1, int p2)
    {
        throw new NotImplementedException();
    }

    public static string PublicStaticMethod()
    {
        throw new NotImplementedException();
    }

    public string PublicGenericMethod<T1, T2>(T1 p1, T2 p2)
    {
        throw new NotImplementedException();
    }

    private class InnerGenericClass<T3, T4>
    {
        private class InnerInnerClass
        {

        }
        public interface InnerGenericInterface<T5, T6>
        {

        }
    }
}
