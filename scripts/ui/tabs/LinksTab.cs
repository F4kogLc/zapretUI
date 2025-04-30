using System.Numerics;
using ImGuiNET;

internal class LinksTab : ITab
{
    public void Render()
    {
        if (!ImGui.BeginTabItem("Links")) return;

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("Zapret UI - GitHub", new Vector2(160, 30)))
        {
            Utils.OpenURL("http://www.github.com/F4kogLc/zapretUI");
        }

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("Zapret UI - Channel", new Vector2(160, 30)))
        {
            Utils.OpenURL("http://www.t.me/zapret_ui");
        }

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("Zapret UI - Chat", new Vector2(160, 30)))
        {
            Utils.OpenURL("http://www.t.me/zapret_ui_chat");
        }

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("Zapret", new Vector2(160, 30)))
        {
            Utils.OpenURL("http://www.github.com/bol-van/zapret");
        }

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("GoodbyeDPI", new Vector2(160, 30)))
        {
            Utils.OpenURL("https://github.com/ValdikSS/GoodbyeDPI");
        }

        ImGui.EndTabItem();
    }
}
