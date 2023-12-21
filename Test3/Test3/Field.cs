using System.Reflection;

namespace Reflector;

/// <summary>
/// class that represents field of class
/// </summary>
public class Field : Member
{
    public string Modifier { get; private set; }
    public bool IsStatic { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }

    /// <summary>
    /// creates instance of 'Field' class
    /// </summary>
    /// <param name="fieldInfo"></param>
    public Field(FieldInfo fieldInfo)
    {
        if (fieldInfo.IsPrivate)
        {
            Modifier = "private";
        }
        else if (fieldInfo.IsPublic)
        {
            Modifier = "public";
        }
        else
        {
            Modifier = "protected";
        }
        IsStatic = fieldInfo.IsStatic;
        Type = GetTypeString(fieldInfo.FieldType);
        Name = fieldInfo.Name;
    }

    /// <summary>
    /// overrides 'ToString' method of object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Modifier + " " +
            (IsStatic ? "static " : "") +
            Type + " " + Name + ";";
    }
}
