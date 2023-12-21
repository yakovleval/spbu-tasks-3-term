using System.Reflection;
using System.Text;

namespace Reflector;

/// <summary>
/// class that represents class or interface
/// </summary>
public class Structure : Member
{
    public string Modifier { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsInterface { get; private set; }
    public string Name { get; private set; }
    private List<Field> _fields = new();
    private List<Constructor> _ctors = new();
    private List<Method> _methods = new();
    private List<Structure> _types = new();
    public IEnumerable<Field> Fields => _fields;
    public IEnumerable<Constructor> Ctors => _ctors;
    public IEnumerable<Method> Methods => _methods;
    public IEnumerable<Structure> Types => _types;
    private readonly int _nestingLevel;

    /// <summary>
    /// creates instance of 'Structure' class
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nestingLevel"></param>
    public Structure(TypeInfo type, int nestingLevel = 0)
    {
        _nestingLevel = nestingLevel;
        if (type.IsPublic || type.IsNestedPublic)
        {
            Modifier = "public";
        }
        else if (type.IsNotPublic)
        {
            Modifier = "internal";
        }
        else if (type.IsNestedFamily)
        {
            Modifier = "protected";
        }
        else
        {
            Modifier = "private";
        }
        IsInterface = type.IsInterface;
        IsStatic = type.IsSealed && type.IsAbstract;
        Name = GetTypeString(type);
        var attrs = BindingFlags.DeclaredOnly |
                         BindingFlags.NonPublic |
                         BindingFlags.Public |
                         BindingFlags.Instance |
                         BindingFlags.Static;
        _fields = type.GetFields(attrs)
                    .Select(m => new Field(m)).ToList();
        _ctors = type.GetConstructors(attrs)
                    .Select(m => new Constructor(m)).ToList();
        _methods = type.GetMethods(attrs)
                    .Select(m => new Method(m)).ToList();
        _types = type.GetNestedTypes(attrs)
                    .Select(m => new Structure(m.GetTypeInfo())).ToList();
    }

    private string Tabs(int nestingLevel)
    {
        string result = "";
        for (int i = 0; i < nestingLevel; i++)
        {
            result += "\t";
        }
        return result;
    }
    public override string ToString()
    {
        StringBuilder result = new();
        result.Append(Tabs(_nestingLevel) + Modifier + " " +
            (IsStatic ? "static " : "") +
            (IsInterface ? "interface " : "class ") +
            Name + "\n");
        result.Append(Tabs(_nestingLevel) + "{\n");
        foreach (var field in _fields)
        {
            result.Append(Tabs(_nestingLevel + 1) + field.ToString() + "\n");
        }
        foreach (var ctor in _ctors)
        {
            result.Append(Tabs(_nestingLevel + 1) + ctor.ToString() + "\n");
        }
        foreach (var method in _methods)
        {
            result.Append(Tabs(_nestingLevel + 1) + method.ToString() + "\n");
        }
        foreach (var type in _types)
        {
            result.Append(Tabs(_nestingLevel + 1) + type.ToString() + "\n");
        }
        result.Append(Tabs(_nestingLevel) + "}\n");
        return result.ToString();
    }
}
