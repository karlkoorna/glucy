using System.Threading;
using Glucy;
using Glucy.Services;
using Glucy.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

ConsoleWindow.Hide();

var config = await Utils.LoadConfig<Config>("Glucy.json");
var builder = WebApplication.CreateBuilder();

builder.WebHost
	.ConfigureKestrel(options => {
		options.AddServerHeader = false;
		options.Limits.MaxRequestBodySize = 1024 * 1024; // 1 MiB
		options.Limits.MaxRequestHeadersTotalSize = 1024; // 1 KiB
	})
	.ConfigureLogging(options => {
		options.SetMinimumLevel(LogLevel.Information);
		options.AddFilter("Microsoft", LogLevel.Warning);

	});

builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options => { options.InvalidModelStateResponseFactory = _ => new StatusCodeResult(StatusCodes.Status400BadRequest); });

builder.Logging.AddSimpleConsole()
	.AddConsoleFormatter<BasicConsoleFormatter, ConsoleFormatterOptions>()
	.AddConsole(options => {
		options.LogToStandardErrorThreshold = LogLevel.Warning;
		options.FormatterName = BasicConsoleFormatter.Name;
	});

builder.Services
	.Configure<ConsoleFormatterOptions>(options => { options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss"; });

builder.Services
	.AddSingleton(config)
	.AddSingleton<TrayService>()
	.AddSingleton<AuthService>()
	.AddSingleton<HistoryService>();

var app = builder.Build();

app.MapControllers();

await app.Services.GetRequiredService<TrayService>().StartAsync(CancellationToken.None);

app.Lifetime.ApplicationStarted.Register(() => {
	app.Logger.LogInformation("Listening on {}", config.Listen);
	app.Logger.LogInformation("Available for writing at http://{}@{}:{}/v1/entries", config.WriteToken, Utils.GetLocalAddress(), config.Port);
	app.Logger.LogInformation("Available for reading at http://{}@{}:{}/v1/entries", config.ReadToken, Utils.GetLocalAddress(), config.Port);
});

app.Run("http://" + config.Listen);
