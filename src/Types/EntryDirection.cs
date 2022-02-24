using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Glucy.Types;

public enum EntryDirection {

	[Display(Name = "▼")]
	[JsonPropertyName("DoubleDown")]
	DoubleDown,

	[Display(Name = "🡫")]
	[JsonPropertyName("SingleDown")]
	SingleDown,

	[Display(Name = "🡮")]
	[JsonPropertyName("FortyFiveDown")]
	DiagonalDown,

	[Display(Name = "–")]
	[JsonPropertyName("NOT COMPUTABLE")]
	Unavailable,

	[Display(Name = "🡪")]
	[JsonPropertyName("Flat")]
	Flat,

	[Display(Name = "🡭")]
	[JsonPropertyName("FortyFiveUp")]
	DiagonalUp,

	[Display(Name = "🡩")]
	[JsonPropertyName("SingleUp")]
	SingleUp,

	[Display(Name = "⯅")]
	[JsonPropertyName("DoubleUp")]
	DoubleUp

}
