using DoaMais.LowStockAlertService.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DoaMais.LowStockAlertService.Utils
{
    public class AdminListJsonConverter : JsonConverter<List<Admin>>
    {
        public override List<Admin>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<Admin>>(ref reader, options) ?? new List<Admin>();
            }
            else
            {
                reader.Skip(); // Ignorar caso seja um valor inesperado
                return new List<Admin>(); // Retorna uma lista vazia se a conversão falhar
            }
        }

        public override void Write(Utf8JsonWriter writer, List<Admin> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

}
