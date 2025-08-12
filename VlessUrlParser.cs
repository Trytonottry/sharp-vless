using System;
using System.Web;

public static class VlessUrlParser
{
    public static VlessConfig Parse(string vlessUrl)
    {
        try
        {
            var uri = new Uri(vlessUrl);
            var config = new VlessConfig();

            config.UserId = uri.UserInfo;
            config.Address = uri.Host;
            config.Port = uri.Port;

            var query = HttpUtility.ParseQueryString(uri.Query);

            config.Sni = query["sni"] ?? uri.Host;
            config.Host = query["host"] ?? config.Sni;
            config.Path = query["path"] ?? "/vless";
            config.AllowInsecure = query["allowInsecure"] == "1" || query["security"] == "insecure";
            config.Security = query["security"] ?? "tls";

            config.PublicKey = query["pbk"];
            config.ShortId = query["sid"];
            config.Fingerprint = query["fp"] ?? "chrome";
            config.SpiderX = query["spx"];
            config.Flow = query["flow"] ?? "";

            return config;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Неверный формат ссылки VLESS", ex);
        }
    }
}

public static VlessClientHandler.ConnectAsync()
{
    Uri uri;
if (_config.Type == "grpc")
{
    var authority = _config.Host != "" ? _config.Host : _config.Sni;
    var headers = $"\"Grpc-Timeout\",\"15s\",\"Host\",\"{authority}\"";
    uri = new Uri($"wss://{_config.Address}:{_config.Port}/VLessTun?ed=2048");
}
else // ws
{
    uri = new Uri($"wss://{_config.Address}:{_config.Port}{_config.Path}");
}
}
