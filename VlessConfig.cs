public class VlessConfig
{
    public string Address { get; set; } = "";
    public int Port { get; set; }
    public string UserId { get; set; } = "";
    public string Sni { get; set; } = "";
    public string Host { get; set; } = "";
    public string Path { get; set; } = "/vless";
    public bool AllowInsecure { get; set; } = false;
    public string Security { get; set; } = "tls";
    public string Type { get; set; } = "ws"; // ws, grpc
    public string? Fingerprint { get; set; }
    public string? PublicKey { get; set; } // для REALITY
    public string? ShortId { get; set; }
    public string? SpiderX { get; set; }
    public string Flow { get; set; } = "";
}