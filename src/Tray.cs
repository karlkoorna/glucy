using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Glucy;

public class Tray {

	private readonly NotifyIcon _notify;
	private readonly Timer _timer;

	public Tray(TimeSpan timeout) {
		_notify = new() { Visible = true };
		_timer = new() { Enabled = true, Interval = (int) timeout.TotalMilliseconds };
		_timer.Elapsed += OnTimeout;
		_timer.Start();
	}

	private void OnTimeout(object? s, EventArgs e) {
		_notify.Icon = null;
	}

	public void Update(double value, string status) {
		_timer.Stop();
		_timer.Start();

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

		g.DrawString(strValue, new("Courier", fontSize, FontStyle.Bold), Brushes.White, new PointF(16, 16), new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
		_notify.Icon = Icon.FromHandle(bmp.GetHicon());
		_notify.Text = status;
	}

}
