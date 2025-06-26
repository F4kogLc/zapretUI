internal class Config
{
    public bool ShowConsole = true;
    public bool AutoQuit = false;
    public bool RunAtStartup = false;
    public bool AlwaysOnTop = true;
    public float BackgroundAlpha = 0.8f;
    public float FontScale = 1f;
    public string ZapretPath = "";
    public string GoodbyeDpiPath = "";
    public string BlockcheckPath = "";
    public string Target = "https://rutracker.org";
    public List<string> AvailableArguments { get; set; } = new();
    public List<string> SelectedArgumentsChain { get; set; } = new();
    public List<Feature> Features { get; set; } = new();
    public List<string> TestArguments { get; set; } = new();
}
