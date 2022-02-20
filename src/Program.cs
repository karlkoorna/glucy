using System;
using Glucy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

var config = await Config.Load("Glucy.json");
var builder = WebApplication.CreateBuilder();

builder.WebHost.ConfigureLogging(options => {
	options.SetMinimumLevel(LogLevel.Information);
	options.AddFilter("Microsoft", LogLevel.Warning);
});

builder.WebHost.ConfigureKestrel(options => {
	options.AddServerHeader = false;
	options.Limits.MaxRequestBodySize = 1024; // 1 KiB
	options.Limits.MaxRequestHeadersTotalSize = 1024; // 1 KiB
});

var app = builder.Build();
var tray = new Tray(TimeSpan.FromMinutes(10));

app.MapPost("/entries", ([FromBody] Entry entry) => {
	// Discard readings older than 8 minutes.
	if (entry.Timestamp < DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(8))) return Results.Accepted();

	var mmol = Math.Round(entry.Sgv / 18F, 1);
	Console.WriteLine($"Received reading of {mmol:0.##} at {entry.Timestamp.ToLocalTime():HH:mm}");
	tray.Update(mmol, $"{mmol:0.##} {Entry.FormatDirection(entry.Direction)}");

	return Results.Ok();
});

Console.WriteLine($"Listening on {config.Listen}");
Console.WriteLine($"Available at http://{Utils.GetLocalAddress()}:{config.Listen.Split(':')[1]}");
app.Run("http://" + config.Listen);
