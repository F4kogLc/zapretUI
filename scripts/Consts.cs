internal class Consts
{
    public static readonly Version VERSION = new(1, 4, 2);

    public static readonly string ZAPRET_PREFIX = "%ZAPRET_PATH%";
    public static readonly string ZAPRET_POSTFIX = "zapret\\zapret-winws\\winws.exe";

    public static readonly string GOODBYE_PREFIX = "%GOODBYE_PATH%";
    public static readonly string GOODBYE_POSTFIX = "zapret\\x86_64\\goodbyedpi.exe";

    public static readonly string BLOCKCHECK_PREFIX = "%BLOCKCHECK_PATH%";
    public static readonly string BLOCKCHECK_POSTFIX = "zapret\\blockcheck\\blockcheck.cmd";

    public static readonly string FAKE_TLS_PREFIX = "%FAKE_TLS%";
    public static readonly string FAKE_TLS_POSTFIX = "zapret\\tls_clienthello_www_google_com.bin";

    public static readonly string FAKE_QUIC_PREFIX = "%FAKE_QUIC%";
    public static readonly string FAKE_QUIC_POSTFIX = "zapret\\quic_initial_www_google_com.bin";

    public static readonly string IPSET_PREFIX = "%IPSET%";
    public static readonly string IPSET_POSTFIX = "zapret\\ipset-cloudflare.txt";

    public static readonly string HOSTLIST_PREFIX = "%HOSTLIST%";
    public static readonly string HOSTLIST_POSTFIX = "zapret\\list-general.txt";

    public static readonly string CONFIG_PATH = Path.Combine(Utils.GetAppPath(), "config.json");

    public static readonly string FONT_PATH = Path.Combine(Utils.GetAppPath(), "fonts", "roboto.ttf");
}
