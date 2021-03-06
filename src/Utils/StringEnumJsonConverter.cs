using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Glucy.Utils;

public class StringEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum {

	private readonly Dictionary<string, T> _toEnum = new();
	private readonly Dictionary<T, string> _toString = new();

	public StringEnumJsonConverter() {
		var type = typeof(T);

		foreach (var value in Enum.GetValues<T>()) {
			var member = type.GetMember(value.ToString())[0];
			var attrs = member
				.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false)
				.Cast<JsonPropertyNameAttribute>();

			foreach (var attr in attrs) {
				_toEnum.Add(attr.Name, value);
				_toString.Add(value, attr.Name);
			}
		}
	}

	public override T Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
		var value = reader.GetString() ?? throw new JsonException("Value cannot be null.");
		if (!_toEnum.TryGetValue(value, out var resultValue)) throw new JsonException("Value not found.");
		return resultValue;
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		if (!_toString.TryGetValue(value, out var resultValue)) throw new JsonException("Value not found.");
		writer.WriteStringValue(resultValue);
	}

}
