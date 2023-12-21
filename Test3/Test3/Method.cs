using System.Reflection;

namespace Reflector;

public class Method : Member
{
    public string Modifier { get; private set; }
    public bool IsStatic { get; private set; }
    public string ReturnType { get; private set; }
    public string Signature { get; private set; }

    public Method(MethodInfo methodInfo)
    {
        if (methodInfo.IsPrivate)
        {
            Modifier = "private";
        }
        else if (methodInfo.IsPublic)
        {
            Modifier = "public";
        }
        else
        {
            Modifier = "protected";
        }
        IsStatic = methodInfo.IsStatic;
        ReturnType = GetTypeString(methodInfo.ReturnType);
        Signature = GetMethodSignatureString(methodInfo);
    }

    private static string GetMethodSignatureString(MethodInfo methodInfo)
    {
        string signature = methodInfo.Name;
        if (methodInfo.IsGenericMethod)
        {
            var genericArguments = methodInfo.GetGenericArguments();
            var genericArgumentsNames = genericArguments.Select(t => GetTypeString(t)).ToArray();
            signature += $"<{String.Join(",", genericArgumentsNames)}>";
        }
        List<string> parametersString = new();
        var parameters = methodInfo.GetParameters();
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
            ReturnType + " " +
            Signature + " { throw new NotImplementedException(); }";
    }
}
