using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdsetManagement.Application.Converters;

public class NullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            
            // Trata string vazia como null
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }

            // Tenta converter string para int
            if (int.TryParse(stringValue, out var value))
            {
                return value;
            }

            throw new JsonException($"Não foi possível converter '{stringValue}' para int.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }

        throw new JsonException($"Tipo inesperado: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}