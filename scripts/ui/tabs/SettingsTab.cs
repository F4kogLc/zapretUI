using System.Numerics;
using ImGuiNET;

internal class SettingsTab : ITab
{
    readonly ConfigManager configManager;
    readonly ProcessLauncher processLauncher;
    readonly ArgumentChainEditor argumentChainEditor;

    public SettingsTab(ConfigManager configManager)
    {
        this.configManager = configManager;
        processLauncher = new(configManager);
        argumentChainEditor = new(configManager);
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Settings")) return;

        RenderRunButton();
        RenderConfigControls();
        RenderFeaturesSettings();
        argumentChainEditor.Render();
        RenderSettings();

        ImGui.EndTabItem();
    }

    void RenderRunButton()
    {
        ImGuiUtils.CenterUIElement(120);

        if (ImGui.Button("Run Zapret", new Vector2(120, 30)))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Console.WriteLine("Run as admin");
                Utils.RestartAsAdmin();
                return;
            }

            SearchDuplicateProcesses();

            processLauncher.RunAntiZapret();
        }

        ImGui.SameLine();

        if (ImGui.Button("Run GoodbyeDPI", new Vector2(120, 30)))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Console.WriteLine("Run as admin");
                Utils.RestartAsAdmin();
                return;
            }

            SearchDuplicateProcesses();

            processLauncher.RunGoodbyeDPI();
        }

        ImGuiUtils.CenterUIElement(60);

        if (ImGui.Button("Run Blockcheck", new Vector2(120, 30)))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Console.WriteLine("Run as admin");
                Utils.RestartAsAdmin();
                return;
            }

            SearchDuplicateProcesses();

            processLauncher.RunBlockcheck();
        }
        ImGuiUtils.Tooltip("После закрытия окна с блокчеком - успешные аргументы будут скопированы во вкладку Console\n\nAfter closing the blockcheck - successful arguments will be copied to the Console tab");

        ImGui.Separator();
    }

    void SearchDuplicateProcesses()
    {
        if (Utils.IsProcessRunning("winws"))
            Console.WriteLine("winws is already running, close it");

        if (Utils.IsProcessRunning("goodbyedpi"))
            Console.WriteLine("goodbyedpi is already running, close it");

        if (Utils.IsProcessRunning("WinDivert64"))
            Console.WriteLine("WinDivert64 is already running, close it");
    }

    void RenderConfigControls()
    {
        ImGuiUtils.CenterUIElement(120);

        if (ImGui.Button("Save", new Vector2(120, 30)))
        {
            configManager.Save();
        }

        ImGui.SameLine();

        if (ImGui.Button("Load", new Vector2(120, 30)))
        {
            configManager.Load();
        }

        ImGui.Separator();
    }

    void RenderFeaturesSettings()
    {
        //string searchFilter = "";
        //ImGui.SetNextItemWidth(250);
        //ImGui.InputTextWithHint("##FeatureSearch", "Search features...", ref searchFilter, 100);

        ImGui.Columns(2, "FeaturesColumns", true);

        for (var i = 0; i < configManager.Config.Features.Count; i++)
        {
            //if (!string.IsNullOrEmpty(searchFilter) &&
            //   !feature.Name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase))
            //    continue;

            var feature = configManager.Config.Features[i];
            ImGui.PushID($"feature_{i}");


            var enabled = feature.IsEnabled;
            ImGui.Checkbox(feature.Name, ref enabled);
            feature.IsEnabled = enabled;

            ImGuiUtils.Tooltip(feature.Tooltip);

            if ((i + 1) % (configManager.Config.Features.Count / 2 + 1) == 0)
                ImGui.NextColumn();

            ImGui.PopID();
        }
        ImGui.Columns(1);

        ImGui.Separator();
    }

    void RenderSettings()
    {
        ImGui.Checkbox("Show Console", ref configManager.Config.ShowConsole);
        ImGuiUtils.Tooltip("Скрывать консоль запрета\n\nHide console of zapret");

        ImGui.Checkbox("Auto Quit", ref configManager.Config.AutoQuit);
        ImGuiUtils.Tooltip("Авто выход при запуске запрета\n\nAuto quit when zapret is started");

        var runAtStartup = configManager.Config.RunAtStartup;
        if (ImGui.Checkbox("Run at Windows startup", ref runAtStartup))
        {
            configManager.Config.RunAtStartup = runAtStartup;
            configManager.Save();
        }
        ImGuiUtils.Tooltip("Запускать программу при старте винды");

        ImGui.Text("Zapret Path");
        ImGui.InputText("##ZapretInput", ref configManager.Config.ZapretPath, 256);

        ImGui.Text("GoodbyeDPI Path");
        ImGui.InputText("##GoodbyeInput", ref configManager.Config.GoodbyeDpiPath, 256);

        ImGui.Text("Blockcheck Path");
        ImGui.InputText("##BlockcheckInput", ref configManager.Config.BlockcheckPath, 256);

        ImGui.Text("Background Transparency");
        ImGui.SliderFloat("##BackgroundAlpha", ref configManager.Config.BackgroundAlpha, 0.1f, 1.0f, "%.2f");
        ImGuiUtils.Tooltip("Прозрачность окна");

        ImGui.Text("Font Scale");
        ImGui.SliderFloat("##FontScale", ref configManager.Config.FontScale, 0.8f, 2.0f, "%.2f");
        ImGuiUtils.Tooltip("Размер шрифта");
    }
}