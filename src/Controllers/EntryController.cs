using System;
using System.Collections.Generic;
using System.Linq;
using Glucy.Services;
using Glucy.Types;
using Glucy.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Glucy.Controllers;

[ApiController]
[Route("/v1/entries")]
public class EntryController : ControllerBase {

	private readonly ILogger _logger;
	private readonly AuthService _authService;
	private readonly TrayService _trayService;
	private readonly HistoryService _historyService;

	public EntryController(ILoggerFactory logger, AuthService authService, TrayService trayService, HistoryService historyService) {
		_logger = logger.CreateLogger("Glucy.Entry");
		_authService = authService;
		_trayService = trayService;
		_historyService = historyService;
	}

	[HttpGet]
	public ActionResult<List<Entry>> Get() {
		if (!_authService.CanRead(Request)) return Unauthorized(null);

		return Ok(_historyService.GetHistory());
	}

	[HttpPost]
	[Consumes("application/json")]
	public ActionResult Post([FromBody] List<Entry> entries) {
		if (!_authService.CanWrite(Request)) return Unauthorized(null);

		var entry = entries.LastOrDefault();
		if (entry == null) return BadRequest();

		// Discard readings in the future or older than 5+3 minutes.
		if (entry.Timestamp > DateTimeOffset.Now) return Accepted();
		if (entry.Timestamp < DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(8))) return Accepted();

		var mmol = Math.Round(entry.Sgv / 18F, 1);
		_historyService.Add(entry);
		_trayService.DrawValue(mmol, $"{mmol:0.0} {entry.Direction.GetDisplayName()}");
		_logger.LogInformation("Received reading {} {}", mmol.ToString("0.0"), entry.Direction.GetDisplayName());

		return Ok();
	}

}
