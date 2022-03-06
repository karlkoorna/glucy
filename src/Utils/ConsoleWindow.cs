using System;
using System.Runtime.InteropServices;

namespace Glucy.Utils;

public class ConsoleWindow {

	private const int SW_HIDE = 0;
	private const int SW_SHOW = 5;

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	private static extern void ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	public static void Hide() {
		ShowWindow(GetConsoleWindow(), SW_HIDE);
	}

	public static void Show() {
		ShowWindow(GetConsoleWindow(), SW_SHOW);
	}

	public static bool IsVisible() {
		return IsWindowVisible(GetConsoleWindow());
	}

}
