using ImGuiNET;

internal interface IElement
{
    void Render();
}

internal interface ITab
{
    void Render();
}

internal class TabSystem
{
    readonly List<ITab> tabs;

    public TabSystem(params ITab[] tabs)
    {
        this.tabs = new List<ITab>(tabs);
    }

    public void Render()
    {
        ImGui.BeginTabBar("##Tabs");

        foreach (var tab in tabs)
        {
            tab.Render();
        }

        ImGui.EndTabBar();
    }
}