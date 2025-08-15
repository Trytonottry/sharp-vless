using System;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Text; // для StringBuilder
using System.Windows.Media; // для SolidColorBrush

namespace VlessClientApp
{
    public partial class MainWindow : Window
    {
        private VlessClientHandler? _client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_client?.IsConnected == true)
            {
                _client.Disconnect();
                UpdateStatus("Отключено", "Red");
                BtnConnect.Content = "Подключить";
            }
            else
            {
                try
                {
                    var config = VlessUrlParser.Parse(TxtLink.Text.Trim());
                    _client = new VlessClientHandler(config);
                    _client.OnLog += Log;

                    UpdateStatus("Подключение...", "Orange");
                    BtnConnect.Content = "Отключить";

                    var success = await _client.ConnectAsync();
                    if (!success)
                    {
                        UpdateStatus("Ошибка подключения", "Red");
                        BtnConnect.Content = "Подключить";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                    UpdateStatus("Ошибка", "Red");
                }
            }
        }

        private void Log(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                TxtLog.Text += msg + "\n";
                TxtLog.ScrollToEnd();
            });
        }

        private void UpdateStatus(string text, string color)
        {
            this.Dispatcher.Invoke(() =>
            {
                TxtStatus.Text = text;
                TxtStatus.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color)!);
            });
        }
    }
}

public partial class MainWindow : Window
{
    private VlessClientHandler? _client;
    private readonly ConfigManager _config;

    public MainWindow(ConfigManager config)
    {
        InitializeComponent();
        _config = config;

        if (!string.IsNullOrEmpty(_config.LastConfig?.Address))
            TxtLink.Text = ReconstructVlessLink(_config.LastConfig);
    }

    public async Task ConnectAsync(VlessConfig? config = null)
    {
        config ??= VlessUrlParser.Parse(TxtLink.Text.Trim());
        _config.LastConfig = config;
        _config.Save();

        _client = new VlessClientHandler(config);
        _client.OnLog += Log;
        UpdateStatus("Подключение...", "Orange");

        var success = await _client.ConnectAsync();
        if (success)
        {
            UpdateStatus("✅ Подключено", "Green");
        }
        else
        {
            UpdateStatus("❌ Ошибка", "Red");
        }
    }

    public void Disconnect()
    {
        _client?.Disconnect();
        UpdateStatus("Отключено", "Gray");
    }

    private string ReconstructVlessLink(VlessConfig c)
    {
        var query = HttpUtility.ParseQueryString("");
        query["security"] = c.Security;
        query["type"] = c.Type;
        query["sni"] = c.Sni;
        query["host"] = c.Host;
        query["path"] = c.Path;
        if (c.AllowInsecure) query["allowInsecure"] = "1";
        if (c.PublicKey != null) query["pbk"] = c.PublicKey;
        if (c.ShortId != null) query["sid"] = c.ShortId;
        if (c.Fingerprint != null) query["fp"] = c.Fingerprint;
        if (c.SpiderX != null) query["spx"] = c.SpiderX;
        if (!string.IsNullOrEmpty(c.Flow)) query["flow"] = c.Flow;

        var sb = new StringBuilder();
        sb.Append($"vless://{c.UserId}@{c.Address}:{c.Port}");
        if (query.Count > 0)
            sb.Append("?" + query.ToString());
        sb.Append($"#Client");
        return sb.ToString();
    }
}