internal class Config
{
    public bool ShowConsole = true;
    public bool AutoQuit = false;
    public bool RunAtStartup = false;
    public float BackgroundAlpha = 0.8f;
    public float FontScale = 1f;
    public List<string> AvailableArguments { get; set; } = new();
    public List<string> SelectedArgumentsChain { get; set; } = new();
    public List<Feature> Features { get; set; } = new();
}
