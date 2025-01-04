using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Habari.Library.Json
{
    internal class ConstantsJsonConverter : JsonConverter<Constants>
    {
        public override Constants? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            Constants constants = new Constants();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return constants;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    reader.Read();
                    Constant? value = JsonSerializer.Deserialize<Constant>(ref reader, options);
                    if (value != null)
                        constants.Add(value);
                }
            }
            return constants;
        }

        public override void Write(Utf8JsonWriter writer, Constants constants, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var constant in constants)
            {
                JsonSerializer.Serialize(writer, constant, options);
            }
            writer.WriteEndArray();
        }
    }
}
