using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DoaMais.Application.Validators
{

    public class InvalidEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? enumText = reader.GetString();
                if (Enum.TryParse(enumText, true, out T result))
                {
                    return result;
                }
            }

            throw new JsonException($"Invalid value for {typeof(T).Name}. Allowed values: {string.Join(", ", Enum.GetNames(typeof(T)))}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
