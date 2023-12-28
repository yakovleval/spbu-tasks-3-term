namespace MyNUnit;

/// <summary>
/// Attribute for marking test methods
/// </summary>
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

/// <summary>
/// Attribute for marking methods which must be invoked before 
/// all test methods in a class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClassAttribute : Attribute { }

/// <summary>
/// Attribute for marking methods which must be invoked after
/// all test methods in a class
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClassAttribute : Attribute { }

/// <summary>
/// Attribute for marking methods which must be invoked before 
/// every test method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeAttribute : Attribute { }

/// <summary>
/// Attribute for marking methods which must be invoked after
/// every test method
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute { }
