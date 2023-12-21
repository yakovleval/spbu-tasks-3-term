using System.Reflection;

namespace Reflector;

public class Constructor : Member
{
    public string Modifier { get; private set; }
    public bool IsStatic { get; private set; }
    public string Signature { get; private set; }

    public Constructor(ConstructorInfo ctorInfo)
    {
        if (ctorInfo.IsPrivate)
        {
            Modifier = "private";
        }
        else if (ctorInfo.IsPublic)
        {
            Modifier = "public";
        }
        else
        {
            Modifier = "protected";
        }
        IsStatic = ctorInfo.IsStatic;
        Signature = GetCtorSignatureString(ctorInfo);
    }

    private static string GetCtorSignatureString(ConstructorInfo ctorInfo)
    {
        string signature = ctorInfo.DeclaringType!.Name;
        if (signature.Contains('`'))
        {
            int index = signature.IndexOf('`');
            signature = signature.Substring(0, index);
        }
        List<string> parametersString = new();
        var parameters = ctorInfo.GetParameters();
        foreach (var parameter in parameters)
        {
            parametersString.Add($"{GetTypeString(parameter.ParameterType)} {parameter.Name}");
        }
        signature += $"({String.Join(", ", parametersString)})";
        return signature;
    }

    public override string ToString()
    {
        return Modifier + " " +
            (IsStatic ? "static " : "") +
            Signature + " { }";
    }
}
