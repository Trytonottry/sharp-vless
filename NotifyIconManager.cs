using System;
using System.Drawing;
using System.Windows.Forms;

public class NotifyIconManager : IDisposable
{
    private readonly NotifyIcon _icon;
    private readonly MainWindow _window;

    public NotifyIconManager(MainWindow window)
    {
        _window = window;
        _icon = new NotifyIcon
        {
            Icon = new Icon("Resources/app.ico"),
            Text = "VLESS Client",
            Visible = true
        };

        _icon.DoubleClick += (s, e) => ShowWindow();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Показать", null, (s, e) => ShowWindow());
        menu.Items.Add("Подключить", null, async (s, e) => await _window.ConnectAsync());
        menu.Items.Add("Отключить", null, (s, e) => _window.Disconnect());
        menu.Items.Add("Выход", null, (s, e) => Application.Exit());

        _icon.ContextMenuStrip = menu;
    }

    private void ShowWindow()
    {
        _window.Show();
        _window.WindowState = WindowState.Normal;
        _window.Activate();
    }

    public void Dispose() => _icon.Dispose();
}