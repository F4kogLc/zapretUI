using Newtonsoft.Json;

internal class ConfigManager
{
    Config config;

    public Config Config => config;

    readonly AutoStartManager autoStartManager;

    public ConfigManager()
    {
        config = new Config();
        autoStartManager = new AutoStartManager();
        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(Consts.CONFIG_PATH))
            {
                var json = File.ReadAllText(Consts.CONFIG_PATH);
                config = JsonConvert.DeserializeObject<Config>(json);
                InitDefaultConfig();
            }
            else
            {
                InitDefaultConfig();
                Save();
            }
        }
        catch (Exception ex)
        {
            HandleConfigError("Error loading config", ex);
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(Consts.CONFIG_PATH, json);
            autoStartManager.SetAutoStart(Config.RunAtStartup);
        }
        catch (Exception ex)
        {
            HandleConfigError("Error saving config", ex);
        }
    }

    void InitDefaultConfig()
    {
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        if (string.IsNullOrEmpty(config.ZapretPath))
            config.ZapretPath = AppDomain.CurrentDomain.BaseDirectory + "zapret\\zapret-winws\\winws.exe";

        if (string.IsNullOrEmpty(config.GoodbyeDpiPath))
            config.GoodbyeDpiPath = AppDomain.CurrentDomain.BaseDirectory + "zapret\\x86_64\\goodbyedpi.exe";

        if (string.IsNullOrEmpty(config.BlockcheckPath))
            config.BlockcheckPath = AppDomain.CurrentDomain.BaseDirectory + "zapret\\blockcheck\\blockcheck.cmd";

        if (config.AvailableArguments.Count == 0)
        {
            config.AvailableArguments.Add("--new");
            config.AvailableArguments.Add("--wf-tcp=80,443");
            config.AvailableArguments.Add("--wf-udp=443,50000-50099");
            config.AvailableArguments.Add("--wf-l3=ipv4");
            config.AvailableArguments.Add("--filter-tcp=80,443");
            config.AvailableArguments.Add("--filter-udp=80,443,50000-50099");
            config.AvailableArguments.Add("--filter-l7=discord,stun");
            config.AvailableArguments.Add("--wssize 1:6");
            config.AvailableArguments.Add("--dpi-desync-ttl=1");
            config.AvailableArguments.Add("--dpi-desync-ttl=2");
            config.AvailableArguments.Add("--dpi-desync-ttl=3");
            config.AvailableArguments.Add("--dpi-desync-ttl=4");
            config.AvailableArguments.Add("--dpi-desync-ttl=5");
            config.AvailableArguments.Add("--dpi-desync-autottl=1");
            config.AvailableArguments.Add("--dpi-desync-autottl=2");
            config.AvailableArguments.Add("--dpi-desync-autottl=3");
            config.AvailableArguments.Add("--dpi-desync-autottl=4");
            config.AvailableArguments.Add("--dpi-desync-autottl=5");
            config.AvailableArguments.Add("--dpi-desync-any-protocol=1");
            config.AvailableArguments.Add("--dpi-desync-cutoff=n5");
            config.AvailableArguments.Add("--dpi-desync-repeats=10");
            config.AvailableArguments.Add("--dpi-desync=fake");
            config.AvailableArguments.Add("--dpi-desync=fakedsplit");
            config.AvailableArguments.Add("--dpi-desync=fakeddisorder");
            config.AvailableArguments.Add("--dpi-desync=fakeddisorder2");
            config.AvailableArguments.Add("--dpi-desync=fake,fakedsplit");
            config.AvailableArguments.Add("--dpi-desync=fake,multidisorder");
            config.AvailableArguments.Add("--dpi-desync=fake,split2");
            config.AvailableArguments.Add("--dpi-desync=disorder");
            config.AvailableArguments.Add("--dpi-desync=multidisorder");
            config.AvailableArguments.Add("--dpi-desync=syndata");
            config.AvailableArguments.Add("--dpi-desync=syndata,disorder2");
            config.AvailableArguments.Add("--dpi-desync=syndata,split2");
            config.AvailableArguments.Add("--dpi-desync=syndata,multidisorder");
            config.AvailableArguments.Add("--dpi-desync-fooling=badseq");
            config.AvailableArguments.Add("--dpi-desync-fooling=md5sig");
            config.AvailableArguments.Add("--dpi-desync-fooling=md5sig,badseq");
            config.AvailableArguments.Add("--dpi-desync-split-seqovl=2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=method+2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1");
            config.AvailableArguments.Add("--dpi-desync-split-pos=2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=3");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1,midsld");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1,sniext+1,host+1,midsld-2,midsld,midsld+2,endhost-1");
            config.AvailableArguments.Add("--dpi-desync-fake-tls-mod=rnd,rndsni,padencap");
            config.AvailableArguments.Add("--dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com");
            config.AvailableArguments.Add($@"--dpi-desync-fake-tls=""{appDirectory + "zapret\\blockcheck\\files\\fake\\tls_clienthello_www_google_com.bin"}""");
            config.AvailableArguments.Add($@"--dpi-desync-fake-quic=""{appDirectory + "zapret\\zapret-winws\\files\\quic_initial_www_google_com.bin"}""");
            config.AvailableArguments.Add($@"--hostlist=""{appDirectory + "zapret\\zapret-winws\\files\\list-youtube.txt"}""");
        }

        if (config.Features.Count == 0)
        {
            config.Features =
            [
                new()
                {
                    Name = "Bypass Method #1",
                    IsEnabled = true,
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50099",
                        "--wf-l3=ipv4",
                        "--dpi-desync=fakeddisorder",
                        "--dpi-desync-ttl=1",
                        "--dpi-desync-autottl=4",
                        "--dpi-desync-split-pos=method+2",
                        "--new",
                        "--dpi-desync=syndata,multidisorder",
                        $@"--dpi-desync-fake-quic=""{appDirectory + "zapret\\zapret-winws\\files\\quic_initial_www_google_com.bin"}""",
                        "--dpi-desync-split-pos=method+2",
                        "--new",
                        "--dpi-desync=multidisorder",
                        "--dpi-desync-split-pos=1,sniext+1,host+1,midsld-2,midsld,midsld+2,endhost-1",
                        "--new"
                    ],
                    Tooltip = "Ютуб работает, дискорд не проверял"
                },
                new()
                {
                    Name = "Bypass Method #2",
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50099",
                        "--wf-l3=ipv4",
                        "--wssize 1:6",
                        "--dpi-desync=multidisorder",
                        "--dpi-desync-split-pos=2",
                    ],
                    Tooltip = "Ютуб работает, дискорд не проверял"
                },
                new()
                {
                    Name = "Bypass Method #3",
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50099",
                        "--wf-l3=ipv4",
                        "--dpi-desync=fakedsplit",
                        "--dpi-desync=fakeddisorder",
                        "--dpi-desync-fooling=md5sig,badseq",
                        "--dpi-desync-split-pos=1,midsld",
                        "--dpi-desync-ttl=4",
                        "--dpi-desync-repeats=11",
                        "--dpi-desync=multidisorder",
                        "--dpi-desync-split-pos=1"
                    ],
                    Tooltip = "Ютуб работает, дискорд не проверял"
                },
                new()
                {
                    Name = "Bypass Method #4",
                    Arguments = [$@"--wf-tcp=80,443 --wf-udp=443,50000-50099 --filter-tcp=80 --dpi-desync=fake,fakedsplit --dpi-desync-autottl=2 --dpi-desync-fooling=md5sig --new --filter-tcp=443 --hostlist=""{appDirectory + "zapret\\zapret-winws\\files\\list-youtube.txt"}"" --dpi-desync=fake,multidisorder --dpi-desync-split-pos=1,midsld --dpi-desync-repeats=11 --dpi-desync-fooling=md5sig --dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com --new --filter-tcp=443 --dpi-desync=fake,multidisorder --dpi-desync-split-pos=midsld --dpi-desync-repeats=6 --dpi-desync-fooling=badseq,md5sig --new --filter-tcp=443 --hostlist=""{appDirectory + "zapret\\zapret-winws\\files\\list-youtube.txt"}"" --dpi-desync=fake --dpi-desync-repeats=11 --dpi-desync-fake-quic=""{appDirectory + "zapret\\files\\quic_initial_www_google_com.bin"}"" --new --filter-tcp=443 --dpi-desync=fake --dpi-desync-repeats=11 --new --filter-udp=50000-50099 --filter-l7=discord,stun --dpi-desync=fake"],
                    Tooltip = "Не работает"
                },
                new()
                {
                    Name = "--dpi-desync=fake",
                    Arguments = ["--dpi-desync=fake"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync=fakeddisorder",
                    Arguments = ["--dpi-desync=fakeddisorder"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync=multidisorder",
                    Arguments = ["--dpi-desync=multidisorder"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync-fooling=md5sig",
                    Arguments = ["--dpi-desync-fooling=md5sig"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync-fooling=badseq,md5sig",
                    Arguments = ["--dpi-desync-fooling=badseq,md5sig"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync-ttl=1",
                    Arguments = ["--dpi-desync-ttl=1"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync-autottl=5",
                    Arguments = ["--dpi-desync-autottl=5"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--dpi-desync-split-pos=method+2",
                    Arguments = ["--dpi-desync-split-pos=method+2"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--wssize 1:6",
                    Arguments = ["--wssize 1:6"],
                    Tooltip = ""
                },
                new()
                {
                    Name = "--new",
                    Arguments = ["--new"],
                    Tooltip = ""
                },
            ];
        }
    }

    void HandleConfigError(string message, Exception ex)
    {
        Console.WriteLine($"{message}: {ex.Message}");
    }
}
