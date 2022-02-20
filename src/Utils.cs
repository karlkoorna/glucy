using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Glucy;

public static class Utils {

	private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

	public static byte[] RandomBytes(int length) {
		var bytes = new byte[length];
		_rng.GetBytes(bytes);
		return bytes;
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

	public static IPAddress GetLocalAddress() {
		using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
		socket.Connect("1.1.1.1", 65000);
		var endpoint = (IPEndPoint) socket.LocalEndPoint!;
		return endpoint.Address;
	}

}
