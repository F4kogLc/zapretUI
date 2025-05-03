using ImGuiNET;
using System.Numerics;

internal class Notification : IElement
{
    readonly List<string> notifications = new();
    readonly object notificationLock = new();
    const float notificationDuration = 7f;
    const float padding = 20f;
    const float xOffset = 20f;
    const float yOffest = 20f;

    public void Render()
    {
        var viewport = ImGui.GetMainViewport();
        var workPos = viewport.WorkPos;
        var workSize = viewport.WorkSize;

        lock (notificationLock)
        {
            for (int i = 0; i < notifications.Count; i++)
            {
                var text = notifications[i];
                var textSize = ImGui.CalcTextSize(text);

                ImGui.SetNextWindowPos(new Vector2(
                    workPos.X + workSize.X - padding - textSize.X - xOffset,
                    workPos.Y + workSize.Y - padding - textSize.Y * (i + 1) - padding * i - yOffest
                ), ImGuiCond.Always);

                ImGui.SetNextWindowSize(new Vector2(textSize.X + 15, textSize.Y + 15));
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 5f);
                ImGui.Begin($"##Notification{i}",
                    ImGuiWindowFlags.NoDecoration |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoSavedSettings |
                    ImGuiWindowFlags.NoInputs |
                    ImGuiWindowFlags.NoNav |
                    ImGuiWindowFlags.NoFocusOnAppearing);

                ImGui.TextColored(new Vector4(1, 1, 0, 1), text);
                ImGui.End();
                ImGui.PopStyleVar();
            }
        }
    }

    public void AddNotification(string message)
    {
        lock (notificationLock)
        {
            notifications.Add(message);
            Task.Delay(TimeSpan.FromSeconds(notificationDuration))
                .ContinueWith(_ => RemoveNotification(message));
        }
    }

    void RemoveNotification(string message)
    {
        lock (notificationLock)
        {
            notifications.Remove(message);
        }
    }
}
