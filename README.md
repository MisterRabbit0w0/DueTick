
<picture>
    <img width="250px" src="./Resources/icon.png" />
</picture>

<h1 align="center">DueTick</h1>

A lightweight desktop deadline reminder tool for Windows.

## Features

- **System Tray Integration**: Runs discreetly in the system tray with easy access via right-click menu.
- **Clipboard Monitoring**: Automatically detects dates and times in your clipboard and suggests creating deadlines.
- **Notifications**: Use native Windows notify.
- **Settings**: Customize notification timing, clipboard monitoring, and widget display.

## Installation

### Prerequisites
- Windows 10 or later
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Building from Source
1. Clone the repository:
   ```bash
   git clone https://github.com/MisterRabbit0w0/DueTick.git
   cd DueTick
   ```

2. Build the project:
   ```bash
   dotnet build --configuration Release
   ```

3. Run the application:
   ```bash
   dotnet run --project DueTick.csproj
   ```

### Downloading Pre-built Binaries
Download the latest release from the [GitHub Releases](https://github.com/MisterRabbit0w0/DueTick/releases) page.

## Usage

1. **Starting the App**: Launch DueTick.exe. It will appear in your system tray.

2. **Adding Deadlines**:
   - Right-click the tray icon and select "Add Deadline".
   - Or copy a date/time to your clipboard, and DueTick will prompt you to create a deadline.

3. **Viewing Deadlines**:
   - Double-click the tray icon or select "View Deadlines" from the context menu.
   - The widget window shows all upcoming deadlines.

4. **Settings**:
   - Right-click the tray icon and select "Settings" to configure:
     - Notification timing
     - Clipboard monitoring
     - Widget display

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built with [.NET 8.0](https://dotnet.microsoft.com/) and [WPF](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- Uses [Newtonsoft.Json](https://www.newtonsoft.com/json) for data serialization
- Uses [Humanizer](https://github.com/Humanizr/Humanizer) for human-readable date formatting
