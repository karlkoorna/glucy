using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Glucy.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace Glucy.Services;

public class TrayService : Form, IHostedService {

	private readonly ILogger _logger;
	private readonly Config _config;

	private readonly NotifyIcon _icon = new();
	private readonly Timer _timer = new();

	public TrayService(ILoggerFactory loggerFactory, Config config) {
		_logger = loggerFactory.CreateLogger("Glucy.Tray");
		_config = config;
	}

	protected override void OnLoad(EventArgs e) {
		Visible = false;
		ShowInTaskbar = false;
		base.OnLoad(e);
	}

	protected override void Dispose(bool isDisposing) {
		if (isDisposing) _icon.Dispose();
		base.Dispose(isDisposing);
	}

	public Task StartAsync(CancellationToken token) {
		var thread = new Thread(OnThread) { IsBackground = true };
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken token) {
		return Task.CompletedTask;
	}

	private void OnThread() {
		_icon.Visible = true;
		_icon.MouseDown += OnClick;
		_icon.ContextMenuStrip = new() {
			Items = {
				new ToolStripMenuItem("Show", null, (_, _) => ConsoleWindow.ShowHide()),
				new ToolStripMenuItem("Exit", null, (_, _) => Environment.Exit(0))
			}
		};

		_timer.Enabled = true;
		_timer.Elapsed += OnTimeout;
		_timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds;

		DrawDot();
		Application.Run(this);
	}

	private void OnClick(object? sender, MouseEventArgs e) {
		_icon.ContextMenuStrip.Items[0].Text = ConsoleWindow.Visible ? "Hide" : "Show";
	}

	private void OnTimeout(object? sender, ElapsedEventArgs e) {
		_logger.LogWarning("Hid tray value after 10 minutes of inactivity");
		DrawDot();
	}

	public void DrawDot() {
		_timer.Stop();

		var bmp = new Bitmap(32, 32);
		var g = Graphics.FromImage(bmp);
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.SmoothingMode = SmoothingMode.HighQuality;
		g.PixelOffsetMode = PixelOffsetMode.HighQuality;

		g.FillEllipse(Brushes.LightGray, new(8, 8, 16, 16));

		_icon.Icon = Icon.FromHandle(bmp.GetHicon());
	}

	public void DrawValue(double value, string status) {
		_timer.Stop();
		_timer.Start();

		var bmp = new Bitmap(32, 32);
		var g = Graphics.FromImage(bmp);
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.SmoothingMode = SmoothingMode.HighQuality;
		g.PixelOffsetMode = PixelOffsetMode.HighQuality;

		string strValue;
		int fontSize;
		Brush brush;

		if (value >= 10) {
			strValue = value.ToString("0");
			fontSize = 19;
		} else {
			strValue = value.ToString("0.0");
			fontSize = 17;
		}

		if (value < _config.LowValue || value >= _config.HighValue) {
			brush = Brushes.Coral;
		} else {
			brush = Brushes.White;
		}

		g.DrawString(strValue, new("Courier", fontSize, FontStyle.Bold), brush, new PointF(16, 16), new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

		_icon.Icon = Icon.FromHandle(bmp.GetHicon());
		_icon.Text = status;
	}

}
