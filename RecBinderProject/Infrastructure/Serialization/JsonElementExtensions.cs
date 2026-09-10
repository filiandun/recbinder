using System.Text.Json;
using System.Diagnostics;


namespace RecBinderProject.Infrastructure.Serialization
{
    public static class JsonElementExtensions
    {
        // TODO удалить и сделать вместо этого TryGetProperty и возвращать null. Кидать исключения - ресурсозатратно
        public static JsonElement GetRequired(this JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var property)) return property;

            throw new JsonException($"Missing required property: {propertyName}");
        }
    }
}
