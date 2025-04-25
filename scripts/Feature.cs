internal class Feature
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string[] Arguments { get; set; }
    public string Tooltip { get; set; } = "";
}
