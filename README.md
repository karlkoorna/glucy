# Glucy

Nightscout Sync target to display the last glucose reading from [xDrip+](https://github.com/NightscoutFoundation/xDrip) in the Windows system tray.

It is highly recommended to only run Glucy within your home network, otherwise setup HTTPS certificates through a reverse proxy.

## Usage

0. Make sure the phone and the computer are connected to the same network.
1. Right-click the system tray icon then left-click "Show" to show the console.
2. In xDrip+ go to `Settings -> Cloud Upload -> Nightscout Sync`.
3. Set `Base URL` to the writing address printed to the console.
