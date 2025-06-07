using ImGuiNET;
using System.Numerics;
using System.Text;

internal class StringWriter : TextWriter
{
    public Action<string> OnWriteLine;
    public override Encoding Encoding => Encoding.UTF8;
    public override void WriteLine(string value) => OnWriteLine?.Invoke(value);
    public override void WriteLine(object value) => OnWriteLine?.Invoke(value?.ToString());
}

internal class OutputLogger : IElement
{
    readonly StringBuilder logBuffer = new();
    bool scrollToBottom;

    public OutputLogger()
    {
        var writer = new StringWriter();
        writer.OnWriteLine += Log;
        Console.SetOut(writer);
    }

    public void Log(string message)
    {
        logBuffer.AppendLine(message);
        scrollToBottom = true;
    }

    public void Render()
    {
        if (ImGui.Button("Copy"))
        {
            ImGui.SetClipboardText(logBuffer.ToString());
        }

        ImGui.SameLine();

        if (ImGui.Button("Clear"))
        {
            logBuffer.Clear();
        }

        ImGui.BeginChild("ScrollingRegion", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()));

        var logText = logBuffer.ToString();

        ImGui.InputTextMultiline(
            "##LogOutput",
            ref logText,
            100000,
            new Vector2(-1, -1),
            ImGuiInputTextFlags.ReadOnly
        );

        if (scrollToBottom)
        {
            ImGui.SetScrollHereY(1.0f);
            scrollToBottom = false;
        }

        ImGui.EndChild();
    }
}
