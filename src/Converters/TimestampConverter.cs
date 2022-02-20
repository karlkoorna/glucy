﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Glucy.Converters;

public class TimestampConverter : JsonConverter<DateTimeOffset> {

	public override DateTimeOffset Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
		return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
	}

	public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
		writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
	}

}
