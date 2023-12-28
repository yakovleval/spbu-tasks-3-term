namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
public class MyTestAttribute : Attribute
{
    public Type? Expected { get; private set; }
    public string? Ignore { get; private set; }
    public MyTestAttribute(Type? expected = null, string? ignore = null)
    {
        Expected = expected;
        Ignore = ignore;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute { }
