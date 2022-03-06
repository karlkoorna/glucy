using System;
using System.Collections.Generic;
using Glucy.Types;
using Microsoft.Extensions.Logging;

namespace Glucy.Services;

public class HistoryService {

	private readonly ILogger _logger;
	private readonly Config _config;
	private readonly SortedList<DateTimeOffset, Entry> _history = new();

	public HistoryService(ILoggerFactory loggerFactory, Config config) {
		_logger = loggerFactory.CreateLogger("Glucy.History");
		_config = config;
	}

	public void Add(Entry entry) {
		if (_history.ContainsKey(entry.Timestamp)) _history.Remove(entry.Timestamp); // Keep latest for same timestamp.
		_history.Add(entry.Timestamp, entry);
		if (_history.Count == _config.HistorySize) _logger.LogInformation("History size reached {}", _history.Values.Count);
		if (_history.Count > _config.HistorySize) _history.RemoveAt(0); // Enforce circular list.
	}

	public IList<Entry> GetHistory() {
		return _history.Values;
	}

}
