using System.Reflection;
using System.Text;

namespace Reflector;

/// <summary>
/// class which generates code for given type
/// </summary>
public static class Reflector
{
    private static string GetStructureString(TypeInfo type)
    {
        var structure = new Structure(type);
        return structure.ToString();
    }

    /// <summary>
    /// prints class to specified file
    /// </summary>
    /// <param name="path">path to file</param>
    /// <param name="someClass">class to print</param>
    public static void PrintStructure(string path, Type someClass)
    {
        var type = someClass.GetTypeInfo();
        var file = File.CreateText(path + type.Name + "Reflected.cs");
        file.Write(GetStructureString(type));
        file.Flush();
        file.Close();
    }

    /// <summary>
    /// prints members of 'a' class which are not present in 'b' class
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string DiffLeftMinusRight(Type a, Type b)
    {
        var left = new Structure(a.GetTypeInfo());
        var right = new Structure(b.GetTypeInfo());
        StringBuilder result = new();
        foreach (var field in left.Fields)
        {
            if (!right.Fields.Contains(field))
            {
                result.Append(field + "\n");
            }
        }
        foreach (var ctor in left.Ctors)
        {
            if (!right.Ctors.Contains(ctor))
            {
                result.Append(ctor + "\n");
            }
        }
        foreach (var method in left.Methods)
        {
            if (!right.Methods.Contains(method))
            {
                result.Append(method + "\n");
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// prints members of 'a' which are not present in 'b' and
    /// members of 'b' which are not present in 'a'
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string DiffClasses(Type a, Type b)
    {
        return DiffLeftMinusRight(a, b) + "\n" + DiffLeftMinusRight(b, a);
    }
}
