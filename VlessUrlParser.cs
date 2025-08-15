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
              var query = QueryHelpers.ParseQuery(uri.Query);

            config.Sni = query["sni"].FirstOrDefault() ?? uri.Host;
            config.Host = query["host"].FirstOrDefault() ?? config.Sni;
            config.Path = query["path"].FirstOrDefault() ?? "/vless";
            config.AllowInsecure = query["allowInsecure"] == "1" || query["security"] == "insecure";
            config.Security = query["security"].FirstOrDefault() ?? "tls";

            config.PublicKey = query["pbk"].FirstOrDefault() ?? string.Empty;
            config.ShortId = query["sid"].FirstOrDefault() ?? string.Empty;
            config.Fingerprint = query["fp"].FirstOrDefault() ?? "chrome";
            config.SpiderX = query["spx"].FirstOrDefault() ?? string.Empty;
            config.Flow = query["flow"].FirstOrDefault() ?? string.Empty;
            config.Type = query["type"].FirstOrDefault() ?? "ws";
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