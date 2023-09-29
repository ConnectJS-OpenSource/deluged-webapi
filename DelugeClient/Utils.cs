using System.Text.Json.Serialization;

internal static class Utils
{
    public static List<String> GetAllJsonPropertyFromType(Type t)
    {
        var type = typeof(JsonPropertyNameAttribute);
        var props = t.GetProperties().Where(prop => Attribute.IsDefined(prop, type)).ToList();
        var propsNames = props.Select(x => x.GetCustomAttributes(type, true).Single()).Cast<JsonPropertyNameAttribute>().Select(x => x.Name);

        return propsNames.ToList();
    }
}