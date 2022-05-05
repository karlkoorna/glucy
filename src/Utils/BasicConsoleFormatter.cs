using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Glucy.Utils;

public sealed class BasicConsoleFormatter : ConsoleFormatter, IDisposable {

	public new const string Name = "basic";

	private readonly IDisposable? _optionsToken;
	private ConsoleFormatterOptions _options;

	public BasicConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options) : base(Name) {
		_optionsToken = options.OnChange(OnOptionsChange);
		_options = options.CurrentValue;
	}

	private void OnOptionsChange(ConsoleFormatterOptions options) {
		_options = options;
	}

	public override void Write<T>(in LogEntry<T> entry, IExternalScopeProvider provider, TextWriter writer) {
		var msg = entry.Formatter?.Invoke(entry.State, entry.Exception);
		if (msg == null) return;

		var timestamp = _options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
		writer.WriteLine($"[{timestamp.ToString(_options.TimestampFormat)}] [{entry.LogLevel.ToString()[0]}] [{entry.Category}] {entry.Exception?.Message ?? msg}");
		if (entry.Exception != null) writer.WriteLine(entry.Exception);
	}

	public void Dispose() {
		_optionsToken?.Dispose();
	}

}
