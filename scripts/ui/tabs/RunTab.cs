using System.Numerics;
using ImGuiNET;

internal class RunTab : ITab
{
    readonly ConfigManager configManager;
    readonly ProcessLauncher processLauncher;
    readonly TestConnection testConnection;

    public RunTab(ConfigManager configManager)
    {
        this.configManager = configManager;
        processLauncher = new(configManager);
        testConnection = new(configManager, processLauncher);
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Run")) return;

        RenderStatus();
        testConnection.Render();

        ImGui.EndTabItem();
    }

    void RenderStatus()
    {
        var isWinwsRunning = Utils.IsProcessRunning("winws");

        if (isWinwsRunning)
        {
            ImGui.TextColored(new Vector4(0, 1, 0, 1), "Zapret Running:");

            ImGui.SameLine();

            foreach (var feature in configManager.Config.Features.Where(f => f.IsEnabled))
            {
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0, 1, 0, 1), feature.Name);
            }

            ImGui.Separator();

            ImGuiUtils.CenterUIElement(120);

            if (ImGui.Button("Restart Zapret", new Vector2(120, 30)))
            {
                if (!Utils.IsRunAsAdmin())
                {
                    Utils.RestartAsAdmin();
                    return;
                }

                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");

                configManager.Load();

                processLauncher.RunZapret();
            }

            ImGui.SameLine();

            if (ImGui.Button("Run GoodbyeDPI", new Vector2(120, 30)))
            {
                if (!Utils.IsRunAsAdmin())
                {
                    Utils.RestartAsAdmin();
                    return;
                }

                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");

                configManager.Load();

                processLauncher.RunGoodbye();
            }

            ImGuiUtils.CenterUIElement(60);

            if (ImGui.Button("Stop", new Vector2(120, 30)))
            {
                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");
            }
        }
        else
        {
            ImGuiUtils.CenterUIElement(120);

            if (ImGui.Button("Run Zapret", new Vector2(120, 30)))
            {
                if (!Utils.IsRunAsAdmin())
                {
                    Utils.RestartAsAdmin();
                    return;
                }

                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");

                configManager.Load();

                processLauncher.RunZapret();
            }
            ImGuiUtils.Tooltip("Перед запуском сначала формируется строка аргументов из выбранных Bypass Method\nПотом Argument Chain Builder, для избегания конфликтов нужно выбрать только один Bypass Method\nИ очистить Argument Chain Builder, либо вы знаете, что делаете и переделали всё под себя, то можно поразному эксперементировать");

            ImGui.SameLine();

            if (ImGui.Button("Run GoodbyeDPI", new Vector2(120, 30)))
            {
                if (!Utils.IsRunAsAdmin())
                {
                    Utils.RestartAsAdmin();
                    return;
                }

                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");

                configManager.Load();

                processLauncher.RunGoodbye();
            }
        }

        ImGui.Separator();
    }
}
