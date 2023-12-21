using System.Reflection;

namespace Reflector;

public class Field : Member
{
    public string Modifier { get; private set; }
    public bool IsStatic { get; private set; }
    public string Type { get; private set; }
    public string Name { get; private set; }

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

    public override string ToString()
    {
        return Modifier + " " +
            (IsStatic ? "static " : "") +
            Type + " " + Name + ";";
    }
}
