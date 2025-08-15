using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Net.Sockets; // для TcpClient, TcpListener
using System.IO;          // для Stream

public class VlessClientHandler
{
    private ClientWebSocket _webSocket = null!;
    private TcpListener _socksProxy = null!;
    private readonly VlessConfig _config;
    private CancellationTokenSource _cts = null!;

    public event Action<string> OnLog;
    public bool IsConnected { get; private set; }

    public VlessClientHandler(VlessConfig config)
    {
        _config = config;
    }

    private void Log(string msg) => OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msg}");

    public async Task<bool> ConnectAsync()
    {
        _cts = new CancellationTokenSource();
        try
        {
            _webSocket = new ClientWebSocket();
            _webSocket.Options.RemoteCertificateValidationCallback = (sender, cert, chain, errors) =>
            {
                if (_config.AllowInsecure) return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            if (!string.IsNullOrEmpty(_config.Host))
                _webSocket.Options.SetRequestHeader("Host", _config.Host);

            var uri = new Uri($"wss://{_config.Address}:{_config.Port}{_config.Path}");
            Log($"Подключение к {uri}");
            await _webSocket.ConnectAsync(uri, _cts.Token);
            Log("✅ Подключено к серверу");

            // Запуск SOCKS5
            _socksProxy = new TcpListener(System.Net.IPAddress.Loopback, 1080);
            _socksProxy.Start();
            Log(".SOCKS5 прокси запущен на 127.0.0.1:1080");
            _ = AcceptSocksClientsAsync();

            IsConnected = true;
            return true;
        }
        catch (Exception ex)
        {
            Log($"❌ Ошибка: {ex.Message}");
            Disconnect();
            return false;
        }
    }

    private async Task AcceptSocksClientsAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var client = await _socksProxy.AcceptTcpClientAsync(_cts.Token);
                _ = HandleSocksClientAsync(client);
            }
            catch { break; }
        }
    }

    private async Task HandleSocksClientAsync(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            var buffer = new byte[2];
            await stream.ReadExactlyAsync(buffer);

            if (buffer[0] != 0x05) return;
            await stream.WriteAsync(new byte[] { 0x05, 0x00 });

            await stream.ReadExactlyAsync(buffer, 0, 2);
            if (buffer[1] != 0x01) return; // CONNECT

            await stream.ReadExactlyAsync(buffer, 0, 1); // reserved

            var addrType = await stream.ReadByteAsync();
            string destHost = "";
            int destPort;

            if (addrType == 0x01)
            {
                var ip = new byte[4];
                await stream.ReadExactlyAsync(ip, 0, 4);
                destHost = new System.Net.IPAddress(ip).ToString();
            }
            else if (addrType == 0x03)
            {
                var len = await stream.ReadByteAsync();
                var host = new byte[len];
                await stream.ReadExactlyAsync(host, 0, len);
                destHost = System.Text.Encoding.UTF8.GetString(host);
            }
            else return;

            var portBuf = new byte[2];
            await stream.ReadExactlyAsync(portBuf, 0, 2);
            destPort = (portBuf[0] << 8) | portBuf[1];

            await stream.WriteAsync(new byte[] { 0x05, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

            // Отправка VLESS-запроса
            var addrBytes = System.Text.Encoding.UTF8.GetBytes(destHost);
            var packet = new List<byte>
            {
                0x00 // версия
            };
            packet.AddRange(Guid.Parse(_config.UserId).ToByteArray());
            packet.Add(0x01); // CONNECT
            packet.Add(0x03); // домен
            packet.Add((byte)addrBytes.Length);
            packet.AddRange(addrBytes);
            packet.Add((byte)(destPort >> 8));
            packet.Add((byte)(destPort & 0xFF));

            await _webSocket.SendAsync(new ArraySegment<byte>(packet.ToArray()), WebSocketMessageType.Binary, true, _cts.Token);

            var wsStream = new WebSocketStream(_webSocket);
            _ = Forward(client.GetStream(), wsStream);
            await Forward(wsStream, client.GetStream());
        }
    }

    private async Task Forward(Stream input, Stream output)
    {
        var buf = new byte[8192];
        try
        {
            int r;
            while ((r = await input.ReadAsync(buf)) > 0)
                await output.WriteAsync(buf.AsMemory(0, r));
        }
        catch { }
        finally
        {
            input?.Close();
            output?.Close();
        }
    }

    public void Disconnect()
    {
        _cts?.Cancel();
        _webSocket?.Abort();
        _socksProxy?.Stop();
        IsConnected = false;
        Log("🔌 Отключено");
    }
}