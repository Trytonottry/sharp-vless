using Application = System.Windows.Application;

public partial class App : Application
{
    private ConfigManager _config = null!;
    private NotifyIconManager? _trayIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        _config = ConfigManager.Load();

        if (_config.StartWithSystem)
            AutoStartManager.SetAutoStart(true);

        var mainWindow = new MainWindow(_config);
        _trayIcon = new NotifyIconManager(mainWindow);

        mainWindow.Closed += (s, e) =>
        {
            _trayIcon?.Dispose();
            Shutdown();
        };

        if (_config.AutoConnect && _config.LastConfig != null)
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Minimized;
            _ = mainWindow.ConnectAsync(_config.LastConfig);
        }
        else
        {
            mainWindow.Show();
        }

        base.OnStartup(e);
    }
}