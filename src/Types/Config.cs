using System;
using System.Text.Json.Serialization;
using Glucy.Utils;

namespace Glucy;

public class Config : IConfig<Config> {

	public string Listen { get; set; } = "0.0.0.0:2143";

	public string WriteToken { get; set; } = Utils.Utils.GenerateToken(16);

	public string ReadToken { get; set; } = Utils.Utils.GenerateToken(16);

	public ushort HistorySize { get; set; } = 10;

	[JsonIgnore]
	public ushort Port;

	public void OnLoad(Config config) {
		config.Port = Convert.ToUInt16(config.Listen.Split(':')[1]);
	}

}
