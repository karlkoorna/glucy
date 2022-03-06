using System.Threading;
using System.Windows.Forms;
using Glucy;
using Glucy.Services;
using Glucy.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

var config = await Utils.LoadConfig<Config>("Glucy.json");
var builder = WebApplication.CreateBuilder();

builder.Services.AddSingleton(config);
builder.Services.AddSingleton<TrayService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<HistoryService>();

builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options => {
		options.InvalidModelStateResponseFactory = _ => new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest }; //
	});

builder.Logging
	.AddConsoleFormatter<BasicConsoleFormatter, BasicConsoleFormatterOptions>()
	.AddConsole(options => {
		options.LogToStandardErrorThreshold = LogLevel.Warning;
		options.FormatterName = BasicConsoleFormatter.Name;
	});

builder.WebHost.ConfigureLogging(options => {
	options.SetMinimumLevel(LogLevel.Information);
	options.AddFilter("Microsoft", LogLevel.Warning);
});

builder.WebHost.ConfigureKestrel(options => {
	options.AddServerHeader = false;
	options.Limits.MaxRequestBodySize = 1048576; // 1 MiB
	options.Limits.MaxRequestHeadersTotalSize = 1024; // 1 KiB
});

var app = builder.Build();

app.MapControllers();
await app.Services.GetRequiredService<TrayService>().StartAsync(CancellationToken.None);

app.Lifetime.ApplicationStarted.Register(() => {
	app.Logger.LogInformation("Listening on {}", config.Listen);
	app.Logger.LogInformation("Available for writing at http://{}@{}:{}/v1/entries", config.WriteToken, Utils.GetLocalAddress(), config.Port);
	app.Logger.LogInformation("Available for reading at http://{}@{}:{}/v1/entries", config.ReadToken, Utils.GetLocalAddress(), config.Port);
});

app.Run("http://" + config.Listen);
