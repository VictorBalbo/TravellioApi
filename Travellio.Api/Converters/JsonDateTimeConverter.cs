using System.Text.Json;
using System.Text.Json.Serialization;

namespace Travellio.Api.Converters;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o) =>
        r.GetDateTime().ToUniversalTime();

    public override void Write(Utf8JsonWriter w, DateTime v, JsonSerializerOptions o) =>
        w.WriteStringValue(v.ToUniversalTime());
}