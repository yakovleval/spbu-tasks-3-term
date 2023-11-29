namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; private set; }
    public string? Ignore { get; private set; }
    public TestAttribute(Type? expected = null, string? ignore = null)
    {
        Expected = expected;
        Ignore = ignore;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class BeforeClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class AfterClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute { }
