using System.Collections.Concurrent;
using ImGuiNET;

internal class ConsoleTab : ITab
{
    readonly OutputLogger logger = new();
    readonly LogParser logParser = new();
    readonly ConcurrentQueue<string> pendingMessages = new();

    public ConsoleTab()
    {
        logParser.NewLogMessage += pendingMessages.Enqueue;
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Console")) return;

        while (pendingMessages.TryDequeue(out string message))
        {
            logger.Log(message);
        }

        logger.Render();

        ImGui.EndTabItem();
    }
}
