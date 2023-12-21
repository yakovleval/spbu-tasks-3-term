public class DummyClass
{
	private Int32 _privateNonStaticField;
	public Int32 _publicNonStaticField;
	protected List<List<List<String>>> _protectedNonStaticField;
	private static Int32 _privateStaticField;
	public static Int32 _publicStaticField;
	protected static Int32 _protectedStaticField;
	public DummyClass() { }
	private static List<List<List<Int32>>> PrivateStaticMethod(Int32 p1, Int32 p2) { throw new NotImplementedException(); }
	public static String PublicStaticMethod() { throw new NotImplementedException(); }
	public String PublicGenericMethod<T1,T2>(T1 p1, T2 p2) { throw new NotImplementedException(); }
	private class InnerGenericClass<T3,T4>
{
	public InnerGenericClass() { }
	private class InnerInnerClass<T3,T4>
{
	public InnerInnerClass() { }
}

	public interface InnerGenericInterface<T3,T4,T5,T6>
{
}

}

}
