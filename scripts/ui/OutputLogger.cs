using ImGuiNET;
using System.Numerics;
using System.Text;

internal class OutputLogger : IElement
{
    readonly StringBuilder logBuffer = new();
    bool scrollToBottom;

    public void AddLog(string message)
    {
        logBuffer.AppendLine($"[{DateTime.Now:HH:mm:ss}] - {message}");
        scrollToBottom = true;
    }

    public void Render()
    {
        ImGui.BeginChild("ScrollingRegion", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()),
            ImGuiChildFlags.None,
            ImGuiWindowFlags.HorizontalScrollbar);

        ImGui.TextUnformatted(logBuffer.ToString());

        if (scrollToBottom)
        {
            ImGui.SetScrollHereY(1.0f);
            scrollToBottom = false;
        }

        ImGui.EndChild();
    }
}
