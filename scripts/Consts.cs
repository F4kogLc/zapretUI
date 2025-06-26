internal class Consts
{
    public static readonly Version VERSION = new(1, 5, 0);

    public static readonly string CONFIG_PATH = Path.Combine(Utils.GetAppPath(), "config.json");

    public static readonly string FONT_PATH = Path.Combine(Utils.GetAppPath(), "fonts", "roboto.ttf");

    public static readonly string LISTS_PATH = Path.Combine(Utils.GetAppPath(), "zapret");

    public static readonly Dictionary<string, string> PATH_REPLACEMENTS = new()
    {
        { "%ZAPRET_PATH%", "zapret\\zapret-winws\\winws.exe" },
        { "%GOODBYE_PATH%", "zapret\\x86_64\\goodbyedpi.exe" },
        { "%BLOCKCHECK_PATH%", "zapret\\blockcheck\\blockcheck.cmd" },
        { "%IANA%", "zapret\\http_iana_org.bin" },
        { "%FAKE_TLS%", "zapret\\tls_clienthello_www_google_com.bin" },
        { "%FAKE_QUIC%", "zapret\\quic_initial_www_google_com.bin" },
        { "%IPSET%", "zapret\\ipset-cloudflare.txt" },
        { "%HOSTLIST%", "zapret\\list-general.txt" }
    };
}
