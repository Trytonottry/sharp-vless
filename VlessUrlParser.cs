// VlessUrlParser.cs
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

public static class VlessUrlParser
{
    public static VlessConfig Parse(string vlessUrl)
    {
        if (string.IsNullOrWhiteSpace(vlessUrl))
            throw new ArgumentException("Ссылка VLESS не может быть пустой");

        try
        {
            var uri = new Uri(vlessUrl);
            if (!uri.Scheme.Equals("vless", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Ожидается схема vless://");

            var config = new VlessConfig();

            config.UserId = uri.UserInfo;
            config.Address = uri.Host;
            config.Port = uri.Port > 0 ? uri.Port : 443;

            if (!string.IsNullOrEmpty(uri.Query))
            {
                var query = HttpUtility.ParseQueryString(uri.Query);

                config.Sni = query["sni"] ?? uri.Host;
                config.Host = query["host"] ?? config.Sni;
                config.Path = query["path"] ?? "/vless";
                config.AllowInsecure = query["allowInsecure"] == "1" || query["security"] == "insecure";
                config.Security = query["security"] ?? "tls";

                config.PublicKey = query["pbk"] ?? string.Empty;
                config.ShortId = query["sid"] ?? string.Empty;
                config.Fingerprint = query["fp"] ?? "chrome";
                config.SpiderX = query["spx"] ?? string.Empty;
                config.Flow = query["flow"] ?? string.Empty;
                config.Type = query["type"] ?? "ws";
            }

            return config;
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("Неверный формат URL");
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Ошибка при парсинге ссылки VLESS", ex);
        }
    }
}