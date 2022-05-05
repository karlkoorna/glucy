using System;
using System.Text.Json.Serialization;
using Glucy.Utils;

namespace Glucy.Types;

public class Entry {

	[JsonPropertyName("device")]
	public string Device { get; set; } = "";

	[JsonPropertyName("rssi")]
	public ushort Rssi { get; set; }

	[JsonPropertyName("sgv")]
	public ushort Sgv { get; set; }

	[JsonPropertyName("delta")]
	public float Delta { get; set; }

	[JsonPropertyName("direction")]
	[JsonConverter(typeof(StringEnumJsonConverter<EntryDirection>))]
	public EntryDirection Direction { get; set; }

	[JsonPropertyName("date")]
	[JsonConverter(typeof(TimestampJsonConverter))]
	public DateTimeOffset Timestamp { get; set; }

}
