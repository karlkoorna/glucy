using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Glucy;

public class Config {

	public string Listen { get; set; } = "0.0.0.0:2143";

	public static async Task<Config> Load(string path) {
		Config? config;

		try {
			await using var stream = File.OpenRead(path);
			config = await JsonSerializer.DeserializeAsync<Config>(stream);
			if (config == null) throw new JsonException("Config file is empty.");
		} catch (Exception) {
			var data = JsonSerializer.Serialize(config = new(), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true });
			await File.WriteAllTextAsync(path, Utils.IndentWithTabs(data, 2) + "\n");
		}

		return config;
	}

}
