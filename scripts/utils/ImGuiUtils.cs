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

    public static void TextCentered(string text, bool isColored = false, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f)
    {
        var availableWidth = ImGui.GetContentRegionAvail().X;
        var textSize = ImGui.CalcTextSize(text);
        var offset = (availableWidth - textSize.X) * 0.5f;

        if (offset > 0)
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offset);

        if (!isColored)
            ImGui.Text(text);
        else
            ImGui.TextColored(new Vector4(r, g, b, a), text);
    }

    public static void CenterUIElement(float width)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float spacing = ImGui.GetStyle().ItemSpacing.X;
        float totalWidth = width * 2 + spacing;
        ImGui.SetCursorPosX((availableWidth - totalWidth) * 0.5f);
    }

    public static void CustomSeparator()
    {
        var drawList = ImGui.GetWindowDrawList();
        var rectMin = ImGui.GetItemRectMin();
        var rectMax = ImGui.GetItemRectMax();

        drawList.AddLine(
            new Vector2(rectMin.X, rectMin.Y + 3),
            new Vector2(rectMax.X, rectMin.Y + 3),
            ImGui.GetColorU32(ImGuiCol.Separator),
            2f
        );
    }

    public static void Tooltip(string text)
    {
        if (ImGui.IsItemHovered() && !string.IsNullOrEmpty(text))
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted(text);
            ImGui.EndTooltip();
        }
    }
}
