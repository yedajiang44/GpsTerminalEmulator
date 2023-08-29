using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jt808TerminalEmulator.Api.Configurations;

public static class JsonSettings
{
    public static IMvcBuilder AddJsonDateTimeConverters(this IMvcBuilder builder)
    {
        return builder
             .AddJsonOptions(x =>
             {
                 x.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                 x.JsonSerializerOptions.Converters.Add(new DatetimeOffsetJsonConverter());
             });
    }
}
internal class DatetimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && typeToConvert == typeof(DateTime) && DateTime.TryParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
internal class DatetimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && typeToConvert == typeof(DateTimeOffset) && DateTimeOffset.TryParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date;
        }
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
    }
}