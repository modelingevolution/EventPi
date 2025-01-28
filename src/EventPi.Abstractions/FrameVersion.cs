using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventPi.Abstractions
{
    public class StreamEventPositionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType) return false;
            return typeToConvert.GetGenericTypeDefinition() == typeof(StreamEventPosition<>);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type valueType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(StreamEventPositionConverter<>).MakeGenericType(valueType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }

    public class StreamEventPositionConverter<T> : JsonConverter<StreamEventPosition<T>>
        where T : struct, IParsable<T>
    {
        public override StreamEventPosition<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string value");

            var value = reader.GetString();
            if (value == null)
                throw new JsonException("String value is null");

            if (!StreamEventPosition<T>.TryParse(value, null, out var result))
                throw new JsonException($"Cannot parse {value} to StreamEventPosition<{typeof(T).Name}>");

            return result;
        }

        public override void Write(Utf8JsonWriter writer, StreamEventPosition<T> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    // ToString = T:Version, we search for last ':' char in the string.
    [JsonConverter(typeof(StreamEventPositionConverterFactory))]
    public readonly record struct StreamEventPosition<T> : IParsable<StreamEventPosition<T>>
        where T : struct, IParsable<T>
    {
        public StreamEventPosition()
        {
            
        }

        public StreamEventPosition(T value, long version = 0)
        {
            Value = value;
            Version = version;
        }
       
        public long Version { get; init; }
        public T Value { get; init; }
        public static StreamEventPosition<T> Parse(string s, IFormatProvider? provider=null)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentNullException(nameof(s));

            var lastColonIndex = s.LastIndexOf(':');
            if (lastColonIndex < 0 || lastColonIndex == s.Length - 1) throw new FormatException("Input string is not in the correct format.");

            var valuePart = s.Substring(0, lastColonIndex);
            var versionPart = s.Substring(lastColonIndex + 1);

            if (!long.TryParse(versionPart, out var version)) throw new FormatException("Version part is not a valid unsigned integer.");

            var value = T.Parse(valuePart, provider);

            return new StreamEventPosition<T> { Value = value, Version = version };
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out StreamEventPosition<T> result)
        {
            result = default;

            if (string.IsNullOrEmpty(s)) return false;

            var lastColonIndex = s.LastIndexOf(':');
            if (lastColonIndex < 0 || lastColonIndex == s.Length - 1) return false;

            var valuePart = s.Substring(0, lastColonIndex);
            var versionPart = s.Substring(lastColonIndex + 1);

            if (!long.TryParse(versionPart, out var version)) return false;

            if (!T.TryParse(valuePart, provider, out var value)) return false;

            result = new StreamEventPosition<T> { Value = value, Version = version };
            return true;
        }

        public override string ToString()
        {
            return $"{Value}:{Version}";
        }
    }
}
