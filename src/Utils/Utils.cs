using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Glucy.Utils;

public static partial class Utils {

	public static async Task<T> LoadConfig<T>(string path) where T : class, IConfig<T>, new() {
		var serializerOptions = new JsonSerializerOptions {
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		T? config;
		try {
			await using var stream = File.OpenRead(path);
			config = await JsonSerializer.DeserializeAsync<T>(stream, serializerOptions);
			if (config == null) throw new JsonException("Config file is empty.");
		} catch (Exception) {
			var data = JsonSerializer.Serialize(config = new(), serializerOptions);
			await File.WriteAllTextAsync(path, IndentWithTabs(data, 2) + "\n");
		}

		config.OnLoad(config);
		return config;
	}

	public static string IndentWithTabs(string str, int width) {
		var builder = new StringBuilder();
		var spaces = -1;

		foreach (var c in str) {
			switch (c) {
				case ' ' when spaces >= 0:
					spaces++;
					break;
				case '\n':
					spaces = 0;
					builder.Append('\n');
					break;
				default:
					if (spaces > 0) builder.Append('\t', spaces / width);
					spaces = -1;
					builder.Append(c);
					break;
			}
		}

		return builder.ToString();
	}

	public static string GetDisplayName<T>(this T value) where T : Enum {
		var member = value.GetType().GetMember(value.ToString());
		if (member.Length == 0) return value.ToString();
		var attribute = member[0].GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
		return attribute?.Name ?? value.ToString();
	}

	public static IPAddress GetLocalAddress() {
		using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
		socket.Connect("1.1.1.1", 65000);
		var endpoint = (IPEndPoint) socket.LocalEndPoint!;
		return endpoint.Address;
	}

	public static string GenerateToken(int length) {
		const string CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		return string.Concat(RandomNumberGenerator.GetBytes(length).Select(x => CHARS[x % CHARS.Length]));
	}

}
