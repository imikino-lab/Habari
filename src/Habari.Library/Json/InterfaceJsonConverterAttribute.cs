using System.Text.Json.Serialization;

namespace Habari.Library.Json;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
internal class InterfaceJsonConverterAttribute : JsonConverterAttribute
{
    public InterfaceJsonConverterAttribute(Type converterType)
    : base(converterType)
    {
    }
}
