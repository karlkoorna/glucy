using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Glucy.Services;

public class AuthService {

	private readonly byte[] _writeSecret;
	private readonly byte[] _readSecret;

	public AuthService(Config config) {
		var sha = SHA1.Create();
		_writeSecret = sha.ComputeHash(Encoding.UTF8.GetBytes(config.WriteToken));
		_readSecret = sha.ComputeHash(Encoding.UTF8.GetBytes(config.ReadToken));
	}

	public bool CanWrite(HttpRequest req) {
		return Can(req, _writeSecret);
	}

	public bool CanRead(HttpRequest req) {
		return Can(req, _readSecret);
	}

	public static bool Can(HttpRequest req, byte[] secret) {
		var headerSecret = FromSecretHeader(req) ?? FromTokenHeader(req);
		return headerSecret != null && CryptographicOperations.FixedTimeEquals(headerSecret, secret);
	}

	public static byte[]? FromSecretHeader(HttpRequest req) {
		var header = req.Headers["api-secret"].FirstOrDefault();
		if (header == null) return null;

		try {
			return Convert.FromHexString(header);
		} catch (FormatException) {
			return null;
		}
	}

	public static byte[]? FromTokenHeader(HttpRequest req) {
		var header = req.Headers["Authorization"].FirstOrDefault();
		if (header == null) return null;

		var token = Convert.FromBase64String(header[6..]);
		if (token[^1] == 0x3A) token = token[..^1];
		return SHA1.Create().ComputeHash(token);
	}

}
