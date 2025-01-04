using Habari.Library.Parameters;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Habari.Library.Json
{
    internal class InputsJsonConverter : JsonConverter<Inputs>
    {
        public override Inputs? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            Inputs inputs = new Inputs();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return inputs;
                }
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    reader.Read();
                    Input? value = JsonSerializer.Deserialize<Input>(ref reader, options);
                    if (value != null)
                        inputs.Add(value);
                }
            }
            return inputs;
        }

        public override void Write(Utf8JsonWriter writer, Inputs inputs, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var input in inputs)
            {
                JsonSerializer.Serialize(writer, input, options);
            }
            writer.WriteEndArray();
        }
    }
}
