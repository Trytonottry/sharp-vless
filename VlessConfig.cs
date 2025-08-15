public class VlessConfig
{
    public string UserId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Sni { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Path { get; set; } = "/vless";
    public bool AllowInsecure { get; set; }
    public string Security { get; set; } = "tls";
    public string PublicKey { get; set; } = string.Empty;
    public string ShortId { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = "chrome";
    public string SpiderX { get; set; } = string.Empty;
    public string Flow { get; set; } = string.Empty;
    public string Type { get; set; } = "ws"; // "ws" или "grpc"
}