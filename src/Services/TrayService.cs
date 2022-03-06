using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Glucy.Services;

public class TrayService : Form, IHostedService {

	private readonly ILogger _logger;
	private readonly NotifyIcon _notify = new();
	private readonly Timer _timer = new();

	private double lastValue;
	private string lastStatus = "";

	public TrayService(ILoggerFactory loggerFactory) {
		_logger = loggerFactory.CreateLogger("Glucy.Tray");

		_notify.Visible = true;
		_timer.Enabled = true;
		_timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;
		_timer.Elapsed += OnTimeout;
	}

	protected override void OnLoad(EventArgs e) {
		Visible = false;
		ShowInTaskbar = false;
		base.OnLoad(e);
	}

	protected override void Dispose(bool isDisposing) {
		if (isDisposing) _notify.Dispose();
		base.Dispose(isDisposing);
	}

	public Task StartAsync(CancellationToken token) {
		var thread = new Thread(() => Application.Run(this)) { IsBackground = true };
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken token) {
		return Task.CompletedTask;
	}

	private void OnTimeout(object? sender, ElapsedEventArgs e) {
		_logger.LogWarning("Hid tray icon after 10 minutes of inactivity");
		Update(lastValue, lastStatus, true);
		_timer.Stop();
	}

	public void Update(double value, string status) {
		Update(value, status, false);
	}

	private void Update(double value, string status, bool fromCache) {
		lastValue = value;
		lastStatus = status;

		if (!fromCache) {
			_timer.Stop();
			_timer.Start();
		}

		var bmp = new Bitmap(32, 32);
		var g = Graphics.FromImage(bmp);
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.SmoothingMode = SmoothingMode.HighQuality;
		g.PixelOffsetMode = PixelOffsetMode.HighQuality;

		string strValue;
		int fontSize;

		if (value >= 10) {
			strValue = value.ToString("0");
			fontSize = 19;
		} else {
			strValue = value.ToString("0.0");
			fontSize = 17;
		}

		g.DrawString(strValue, new("Courier", fontSize, FontStyle.Bold), fromCache ? Brushes.Coral : Brushes.White, new PointF(16, 16), new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

		_notify.Icon = Icon.FromHandle(bmp.GetHicon());
		_notify.Text = status;
	}

}
