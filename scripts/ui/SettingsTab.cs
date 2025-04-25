using System.Numerics;
using ImGuiNET;

internal class SettingsTab : ITab
{
    readonly ConfigManager configManager;
    readonly ProcessLauncher processLauncher;

    int selectedAvailableArgIndex = -1;

    public SettingsTab(ConfigManager configManager)
    {
        this.configManager = configManager;
        processLauncher = new(configManager);
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Settings")) return;

        RenderRunButton();
        RenderConfigControls();
        RenderFeaturesSettings();
        RenderArgumentChainEditor();
        RenderSettings();

        ImGui.EndTabItem();
    }

    void RenderRunButton()
    {
        ImGuiUtils.CenterUIElement(120);

        if (ImGui.Button("Start Anti Zapret", new Vector2(120, 30)))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Console.WriteLine("Run as admin");
                Utils.RestartAsAdmin();
                return;
            }
            processLauncher.RunAntiZapret();
        }

        ImGui.SameLine();

        if (ImGui.Button("Start Blockcheck", new Vector2(120, 30)))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Console.WriteLine("Run as admin");
                Utils.RestartAsAdmin();
                return;
            }
            processLauncher.RunBlockcheck();
        }

        ImGui.Separator();
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

    void RenderArgumentChainEditor()
    {
        ImGui.Text("Argument Chain Builder");

        var availableArgs = configManager.Config.AvailableArguments.ToArray();

        if (ImGui.Combo("##Chain", ref selectedAvailableArgIndex, availableArgs, availableArgs.Length))
        {
            // при выборе аргумента
        }

        ImGui.SameLine();

        if (ImGui.Button("Add to Chain") && selectedAvailableArgIndex >= 0)
        {
            var selectedArg = availableArgs[selectedAvailableArgIndex];
            configManager.Config.SelectedArgumentsChain.Add(selectedArg);
        }

        ImGui.BeginChild("ChainList", new Vector2(0, 250));

        // доступная ширина
        float currentX = 0;
        float currentY = 0;
        var availableWidth = ImGui.GetContentRegionAvail().X;
        var itemSpacing = ImGui.GetStyle().ItemSpacing.X;
        var lineHeight = ImGui.GetTextLineHeightWithSpacing();

        ImGui.BeginGroup();
        for (int i = 0; i < configManager.Config.SelectedArgumentsChain.Count; i++)
        {
            var arg = configManager.Config.SelectedArgumentsChain[i];

            // размер текста
            var textSize = ImGui.CalcTextSize(arg);
            var buttonSize = new Vector2(20, textSize.Y);
            var totalWidth = textSize.X + buttonSize.X + itemSpacing * 2;

            // проверка на необходимость переноса
            if (currentX + totalWidth > availableWidth && currentX > 0)
            {
                currentX = 0;
                currentY += lineHeight;
            }

            ImGui.SetCursorPos(new Vector2(currentX, currentY));

            ImGui.Text(arg);

            // позиционирование кнопки удаления
            ImGui.SetCursorPos(new Vector2(
                currentX + textSize.X + itemSpacing / 2,
                currentY + (textSize.Y - buttonSize.Y) / 2
            ));

            ImGui.PushID($"remove_{i}");
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.1f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
            if (ImGui.Button("X", buttonSize))
            {
                configManager.Config.SelectedArgumentsChain.RemoveAt(i);
                i--;
            }
            ImGui.PopStyleColor(2);
            ImGui.PopID();

            ImGui.SameLine();
            currentX += totalWidth + itemSpacing;
        }
        ImGui.EndGroup();

        ImGui.EndChild();

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

        ImGui.Text("Background Transparency");
        ImGui.SliderFloat("##BackgroundAlpha", ref configManager.Config.BackgroundAlpha, 0.1f, 1.0f, "%.2f");
        ImGuiUtils.Tooltip("Прозрачность окна");

        ImGui.Text("Font Scale");
        ImGui.SliderFloat("##FontScale", ref configManager.Config.FontScale, 0.8f, 2.0f, "%.2f");
        ImGuiUtils.Tooltip("Размер шрифта");
    }
}