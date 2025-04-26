using ImGuiNET;

internal class ExitTab : ITab
{
    public void Render()
    {
        if (!ImGui.BeginTabItem("X")) return;

        Environment.Exit(0);

        ImGui.EndTabItem();
    }
}
