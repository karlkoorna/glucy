using System;
using System.Text.Json.Serialization;
using Glucy.Converters;

namespace Glucy;

public class Entry {

	public enum EntryDirection {

		[JsonPropertyName("DoubleUp")]
		DoubleUp,

		[JsonPropertyName("SingleUp")]
		SingleUp,

		[JsonPropertyName("FortyFiveUp")]
		DiagonalUp,

		[JsonPropertyName("Flat")]
		Flat,

		[JsonPropertyName("FortyFiveDown")]
		DiagonalDown,

		[JsonPropertyName("SingleDown")]
		SingleDown,

		[JsonPropertyName("DoubleDown")]
		DoubleDown,

		[JsonPropertyName("NOT COMPUTABLE")]
		Unavailable

	}

	[JsonPropertyName("device")]
	public string Device { get; set; } = "";

	[JsonPropertyName("sgv")]
	public int Sgv { get; set; }

	[JsonPropertyName("direction")]
	[JsonConverter(typeof(EnumConverter<EntryDirection>))]
	public EntryDirection Direction { get; set; }

	[JsonPropertyName("date")]
	[JsonConverter(typeof(TimestampConverter))]
	public DateTimeOffset Timestamp { get; set; }

	public static string FormatDirection(EntryDirection dir) {
		return dir switch {
			EntryDirection.DoubleUp => "🡩",
			EntryDirection.SingleUp => "🡩",
			EntryDirection.DiagonalUp => "🡭",
			EntryDirection.Flat => "🡪",
			EntryDirection.DiagonalDown => "🡮",
			EntryDirection.SingleDown => "🡫",
			EntryDirection.DoubleDown => "🡫",
			EntryDirection.Unavailable => "–",
			_ => "…"
		};
	}

}
