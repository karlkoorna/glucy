using Microsoft.Extensions.Logging.Console;

namespace Glucy.Utils;

public class BasicConsoleFormatterOptions : ConsoleFormatterOptions {

	public new string TimestampFormat { get; set; } = "HH:mm:ss";

}
