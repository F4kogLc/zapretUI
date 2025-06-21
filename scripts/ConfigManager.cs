using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

internal class MyJsonConverter : JsonConverter
{
    readonly string appDirectory;

    public MyJsonConverter()
    {
        appDirectory = Utils.GetAppPath();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is string stringValue)
        {
            var result = stringValue;

            foreach (var replacement in Consts.PATH_REPLACEMENTS)
            {
                var fullPath = Path.Combine(appDirectory, replacement.Value);
                var path2 = $"\"{fullPath}\"";
                result = result.Replace(path2, replacement.Key);
                result = result.Replace(fullPath, replacement.Key);
            }

            writer.WriteValue(result);
        }
        else
        {
            writer.WriteValue(value);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value is not string value)
            return reader.Value;

        var result = value;

        foreach (var replacement in Consts.PATH_REPLACEMENTS)
        {
            if (result.Contains(replacement.Key))
            {
                var fullPath = Path.Combine(appDirectory, replacement.Value);
                var path2 = $"\"{fullPath}\"";
                result = result.Replace(replacement.Key, path2);
            }
        }

        return result;
    }

    public override bool CanConvert(Type objectType) => objectType == typeof(string);
}

internal class ConfigManager
{
    Config config;

    public Config Config => config;

    readonly AutoStartManager autoStartManager;
    readonly JsonSerializerSettings jsonSettings;
    readonly Notification notification;

    public ConfigManager(Notification notification)
    {
        this.notification = notification;

        config = new Config();
        autoStartManager = new AutoStartManager();
        jsonSettings = new JsonSerializerSettings
        {
            MaxDepth = 128,
            TypeNameHandling = TypeNameHandling.None,
            MissingMemberHandling = MissingMemberHandling.Error,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new MyJsonConverter() },
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableInterface = true
            }
        };
        Load();
        CheckNewVersion();
    }

    async void CheckNewVersion()
    {
        try
        {
            if (await VersionChecker.IsUpdateAvailable())
            {
                //var latest = await VersionChecker.GetLatestGitHubVersion();

                notification.AddNotification("Available new version! Get it from GitHub");
            }
        }
        catch (Exception ex)
        {
            notification.AddNotification($"Version check failed: {ex.Message}");
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(Consts.CONFIG_PATH))
            {
                var fileInfo = new FileInfo(Consts.CONFIG_PATH);

                if (fileInfo.Length > 1024 * 1024) // 1MB
                {
                    File.Delete(Consts.CONFIG_PATH);
                    InitDefaultConfig();
                    Save();

                    notification.AddNotification("Config file was too large and recreated");
                }

                var json = File.ReadAllText(Consts.CONFIG_PATH);
                config = JsonConvert.DeserializeObject<Config>(json, jsonSettings);

                UpdateWindow();
                InitDefaultConfig();

                notification.AddNotification("Loaded");
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
            var saveSettings = new JsonSerializerSettings
            {
                MaxDepth = 128,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new MyJsonConverter() },
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableInterface = true
                },
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(config, saveSettings);

            File.WriteAllText(Consts.CONFIG_PATH, json);
            autoStartManager.SetAutoStart(Config.RunAtStartup);

            notification.AddNotification("Saved");
        }
        catch (Exception ex)
        {
            HandleConfigError("Error saving config", ex);
        }
    }

    // страшный костыль
    async void UpdateWindow()
    {
        await Task.Delay(1000);

        if (!config.AlwaysOnTop)
            WinAPI.SetTopWindow(false);
    }

    void InitDefaultConfig()
    {
        if (string.IsNullOrEmpty(config.ZapretPath))
            config.ZapretPath = "%ZAPRET_PATH%";

        if (string.IsNullOrEmpty(config.GoodbyeDpiPath))
            config.GoodbyeDpiPath = "%GOODBYE_PATH%";

        if (string.IsNullOrEmpty(config.BlockcheckPath))
            config.BlockcheckPath = "%BLOCKCHECK_PATH%";

        if (config.AvailableArguments.Count == 0)
        {
            config.AvailableArguments.Add("--new");
            config.AvailableArguments.Add("--wf-tcp=80,443");
            config.AvailableArguments.Add("--wf-udp=443,50000-50099");
            config.AvailableArguments.Add("--wf-l3=ipv4");
            config.AvailableArguments.Add("--filter-tcp=80");
            config.AvailableArguments.Add("--filter-tcp=443");
            config.AvailableArguments.Add("--filter-udp=80");
            config.AvailableArguments.Add("--filter-udp=443");
            config.AvailableArguments.Add("--filter-udp=50000-50099");
            config.AvailableArguments.Add("--filter-l7=discord,stun");
            config.AvailableArguments.Add("--filter-l7=discord");
            config.AvailableArguments.Add("--filter-l7=stun");
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
            config.AvailableArguments.Add("--dpi-desync-cutoff=n2");
            config.AvailableArguments.Add("--dpi-desync-cutoff=n5");
            config.AvailableArguments.Add("--dpi-desync-repeats=5");
            config.AvailableArguments.Add("--dpi-desync-repeats=8");
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
            config.AvailableArguments.Add("--dpi-desync-fooling=badseq,hopbyhop2");
            config.AvailableArguments.Add("--dpi-desync-fooling=md5sig");
            config.AvailableArguments.Add("--dpi-desync-fooling=md5sig,badseq");
            config.AvailableArguments.Add("--dpi-desync=split2");
            config.AvailableArguments.Add("--dpi-desync-split-seqovl=2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=method+2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1");
            config.AvailableArguments.Add("--dpi-desync-split-pos=2");
            config.AvailableArguments.Add("--dpi-desync-split-pos=3");
            config.AvailableArguments.Add("--dpi-desync-split-pos=midsld");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1,midsld");
            config.AvailableArguments.Add("--dpi-desync-split-pos=1,sniext+1,host+1,midsld-2,midsld,midsld+2,endhost-1");
            config.AvailableArguments.Add("--dpi-desync-fake-tls-mod=rnd,rndsni,padencap");
            config.AvailableArguments.Add("--dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com");
            config.AvailableArguments.Add("--dpi-desync-fake-unknown-udp=%FAKE_QUIC%");
            config.AvailableArguments.Add("--dpi-desync-fake-tls=%FAKE_TLS%");
            config.AvailableArguments.Add("--dpi-desync-fake-quic=%FAKE_TLS%");
            config.AvailableArguments.Add("--dpi-desync-split-seqovl-pattern=%FAKE_TLS%");
            config.AvailableArguments.Add("--ipset=%IPSET%");
            config.AvailableArguments.Add("--hostlist=%HOSTLIST%");
        }

        if (config.Features.Count == 0)
        {
            config.Features =
            [
                new()
                {
                    Name = "Bypass Method #1",
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
                        "--dpi-desync-fake-quic=%FAKE_QUIC%",
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
                    IsEnabled = true,
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
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50100",
                        "--filter-udp=443",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fake-quic=%FAKE_QUIC%",
                        "--new",
                        "--filter-udp=50000-50100",
                        "--filter-l7=discord,stun",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--new",
                        "--filter-tcp=80",
                        "--hostlist=%FAKE_QUIC%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-autottl=2",
                        "--dpi-desync-fooling=md5sig",
                        "--new",
                        "--filter-tcp=443",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=split",
                        "--dpi-desync-split-pos=1",
                        "--dpi-desync-autottl",
                        "--dpi-desync-repeats=8",
                        "--dpi-desync-fooling=badseq",
                        "--new",
                        "--filter-udp=443",
                        "--ipset=%IPSET%",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fake-quic=%FAKE_QUIC%",
                        "--new",
                        "--filter-tcp=80",
                        "--ipset=%IPSET%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-autottl=2",
                        "--dpi-desync-fooling=md5sig",
                        "--new",
                        "--filter-tcp=443",
                        "--ipset=%IPSET%",
                        "--dpi-desync=split",
                        "--dpi-desync-split-pos=1",
                        "--dpi-desync-autottl",
                        "--dpi-desync-repeats=8",
                        "--dpi-desync-fooling=badseq",
                    ],
                    Tooltip = "Ютуб работает, дискорд не проверял (для обхода бана - нужно вносить адреса сайтов в list-general)"
                },
                new()
                {
                    Name = "Bypass Method #5",
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50100",
                        "--filter-udp=443",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fake-quic=%FAKE_QUIC%",
                        "--new",
                        "--filter-udp=50000-50100",
                        "--filter-l7=discord,stun",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--new",
                        "--filter-tcp=80",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-autottl=2",
                        "--dpi-desync-fooling=md5sig",
                        "--new",
                        "--filter-tcp=443",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-split-pos=1",
                        "--dpi-desync-autottl",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fooling=md5sig",
                        "--dpi-desync-fake-tls=%FAKE_TLS%",
                        "--new",
                        "--filter-udp=443",
                        "--ipset=%IPSET%",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fake-quic=%FAKE_QUIC%",
                        "--new",
                        "--filter-tcp=80",
                        "--ipset=%IPSET%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-autottl=2",
                        "--dpi-desync-fooling=md5sig",
                        "--new",
                        "--filter-tcp=443",
                        "--ipset=%IPSET%",
                        "--dpi-desync=fake,split2",
                        "--dpi-desync-split-pos=1",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fooling=md5sig",
                        "--dpi-desync-fake-tls=%FAKE_TLS%"
                    ],
                    Tooltip = "У меня не работает (для обхода бана - нужно вносить адреса сайтов в list-general)",
                },
                new()
                {
                    Name = "Bypass Method #6",
                    Arguments = ["--wf-tcp=80,443 --wf-udp=443,50000-50099 --filter-tcp=80 --dpi-desync=fake,fakedsplit --dpi-desync-autottl=2 --dpi-desync-fooling=md5sig --new --filter-tcp=443 --hostlist=%HOSTLIST% --dpi-desync=fake,multidisorder --dpi-desync-split-pos=1,midsld --dpi-desync-repeats=11 --dpi-desync-fooling=md5sig --dpi-desync-fake-tls-mod=rnd,dupsid,sni=www.google.com --new --filter-tcp=443 --dpi-desync=fake,multidisorder --dpi-desync-split-pos=midsld --dpi-desync-repeats=6 --dpi-desync-fooling=badseq,md5sig --new --filter-tcp=443 --hostlist=%HOSTLIST% --dpi-desync=fake --dpi-desync-repeats=11 --dpi-desync-fake-quic=%FAKE_QUIC% --new --filter-tcp=443 --dpi-desync=fake --dpi-desync-repeats=11 --new --filter-udp=50000-50099 --filter-l7=discord,stun --dpi-desync=fake"],
                    Tooltip = "Не работает"
                },
                new()
                {
                    Name = "Bypass Method #7",
                    Arguments =
                    [
                        "--wf-tcp=80,443",
                        "--wf-udp=443,50000-50099",
                        "--filter-udp=443",
                        "--dpi-desync=fakedsplit",
                        "--dpi-desync-autottl=2",
                        "--dpi-desync-fooling=md5sig",
                        "--new",
                        "--filter-tcp=443",
                        "--hostlist=%HOSTLIST%",
                        "--dpi-desync=split2",
                        "--dpi-desync-split-seqovl=652",
                        "--dpi-desync-split-pos=2",
                        "--dpi-desync-split-seqovl-pattern=%FAKE_TLS%",
                        "--new",
                        "--filter-tcp=443",
                        "--ipset=%IPSET%",
                        "--dpi-desync=split2",
                        "--dpi-desync-split-seqovl=652",
                        "--dpi-desync-split-pos=2",
                        "--dpi-desync-split-seqovl-pattern=%FAKE_TLS%",
                        "--new",
                        "--filter-tcp=443",
                        "--dpi-desync=multidisorder",
                        "--dpi-desync-split-pos=midsld",
                        "--dpi-desync-repeats=6",
                        "--dpi-desync-fooling=badseq,md5sig",
                        "--new",
                        "--filter-udp=443",
                        "--dpi-desync=fake",
                        "--dpi-desync-repeats=11",
                        "--new",
                        "--filter-udp=50000-50099",
                        "--filter-l7=discord,stun",
                        "--dpi-desync=fake"
                    ],
                    Tooltip = "Ютуб работает, дискорд не проверял"
                },
            ];
        }
    }

    void HandleConfigError(string message, Exception ex)
    {
        Console.WriteLine($"{message}: {ex.Message}");
        notification.AddNotification($"{message}: {ex.Message}");
    }
}
