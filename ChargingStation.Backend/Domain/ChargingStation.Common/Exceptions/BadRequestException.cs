using System.Collections;
using System.Reflection;

namespace ChargingStation.Common.Exceptions;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException()
    { }

    public BadRequestException(string entityTypeName, object entity)
        : base($"{entityTypeName} with such parameters already exists. Fields: {GetFields(entity)}")
    { }

    public BadRequestException(string message) : base(message)
    { }

    private static string GetFields(object entity)
    {
        PropertyInfo[] properties = entity.GetType().GetProperties();
        string fields = string.Join(", ", properties.Select(p => $"{p.Name} = {GetValueAsString(p.GetValue(entity))}"));
        return "{" + fields + "}";
    }

    private static string? GetValueAsString(object? value)
    {
        if (value == null)
            return "null";

        if (value is string)
            return $"\"{value}\"";

        if (value is DateTime)
            return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");

        if (value is IEnumerable enumerableValue)
        {
            var items = enumerableValue.Cast<object>().Select(GetValueAsString);
            return $"[{string.Join(", ", items)}]";
        }

        return value.ToString();
    }
}