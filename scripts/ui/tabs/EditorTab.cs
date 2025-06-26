using ImGuiNET;

internal class EditorTab : ITab
{
    readonly BypassEditor bypassEditor;
    readonly ListEditor listEditor;

    public EditorTab(ConfigManager configManager)
    {
        bypassEditor = new(configManager);
        listEditor = new();
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Editor")) return;

        if (ImGui.BeginTabBar("##EditorTabs"))
        {
            if (ImGui.BeginTabItem("Bypass Editor"))
            {
                bypassEditor.Render();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("List Editor"))
            {
                listEditor.Render();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        ImGui.EndTabItem();
    }
}
