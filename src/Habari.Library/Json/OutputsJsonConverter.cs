using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Habari.Library.Json
{
    internal class OutputsJsonConverter : JsonConverter<Outputs>
    {
        public override Outputs? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            Outputs outputs = new Outputs();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return outputs;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    reader.Read();
                    Output? value = JsonSerializer.Deserialize<Output>(ref reader, options);
                    if (value != null)
                        outputs.Add(value);
                }
            }
            return outputs;
        }

        public override void Write(Utf8JsonWriter writer, Outputs outputs, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var output in outputs)
            {
                JsonSerializer.Serialize(writer, output, options);
            }
            writer.WriteEndArray();
        }
    }
}
