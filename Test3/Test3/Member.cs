namespace Reflector;

/// <summary>
/// class that represents member of class 
/// </summary>
public class Member
{
    protected static string GetTypeString(Type type)
    {
        string typeString = type.Name;
        if (typeString.Contains('`'))
        {
            int index = typeString.IndexOf('`');
            typeString = typeString.Substring(0, index);
        }
        var genericArguments = type.GetGenericArguments();
        if (genericArguments.Length > 0)
        {
            var genericArgumentsString = genericArguments.Select(t => GetTypeString(t)).ToArray();
            typeString += $"<{String.Join(",", genericArgumentsString)}>";
        }
        return typeString;
    }

    /// <summary>
    /// overrides 'Equals' method of Object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        return ToString() == obj.ToString();
    }

    /// <summary>
    /// overrides 'GetHashCode' method of Object
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return ToString()!.GetHashCode();
    }
}
