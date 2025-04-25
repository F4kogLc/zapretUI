using ImGuiNET;
using System.Numerics;

internal static class ImGuiUtils
{
    /*
    public static void CenteredButton(string text, Action action, Vector2? size = null)
    {
        var buttonSize = size ?? new Vector2(120, 30);
        var availableWidth = ImGui.GetContentRegionAvail().X;
        var offset = (availableWidth - buttonSize.X) * 0.5f;

        if (offset > 0)
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offset);

        if (ImGui.Button(text, buttonSize))
        {
            action?.Invoke();
        }
    }
    */

    public static void CenterUIElement(float width)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float spacing = ImGui.GetStyle().ItemSpacing.X;
        float totalWidth = width * 2 + spacing;
        ImGui.SetCursorPosX((availableWidth - totalWidth) * 0.5f);
    }

    public static void Tooltip(string text)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(text);
            ImGui.EndTooltip();
        }
    }
}
