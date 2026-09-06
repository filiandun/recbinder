using System.Text.Json;


namespace RecBinderProject.Infrastructure.Serialization
{
    public static class JsonElementExtensions
    {
        public static JsonElement GetRequired(this JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property)) return property;

            throw new JsonException($"Missing required property: {propertyName}");
        }
    }
}
