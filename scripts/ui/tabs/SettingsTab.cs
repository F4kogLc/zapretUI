﻿using System.Numerics;
using ImGuiNET;

internal class SettingsTab : ITab
{
    readonly ConfigManager configManager;
    readonly ArgumentChainEditor argumentChainEditor;

    public SettingsTab(ConfigManager configManager)
    {
        this.configManager = configManager;
        argumentChainEditor = new(configManager);
    }

    public void Render()
    {
        if (!ImGui.BeginTabItem("Settings")) return;

        RenderConfigControls();
        RenderFeaturesSettings();
        argumentChainEditor.Render();
        RenderSettings();

        ImGui.EndTabItem();
    }

    void RenderConfigControls()
    {
        ImGuiUtils.CenterUIElement(120);

        if (ImGui.Button("Save", new Vector2(120, 30)))
        {
            configManager.Save();
        }
        ImGuiUtils.Tooltip("Сохранить текущие настройки в config.json");

        ImGui.SameLine();

        if (ImGui.Button("Load", new Vector2(120, 30)))
        {
            configManager.Load();
        }
        ImGuiUtils.Tooltip("Загрузить настройки из config.json");

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

        var alwaysOnTop = configManager.Config.AlwaysOnTop;
        if (ImGui.Checkbox("Always on Top", ref alwaysOnTop))
        {
            configManager.Config.AlwaysOnTop = alwaysOnTop;

            if (alwaysOnTop)
                WinAPI.SetTopWindow(true);
            else
                WinAPI.SetTopWindow(false);
        }
        ImGuiUtils.Tooltip("Отображение программы поверх всего");

        var runAtStartup = configManager.Config.RunAtStartup;
        if (ImGui.Checkbox("Run at Windows startup", ref runAtStartup))
        {
            configManager.Config.RunAtStartup = runAtStartup;
            configManager.Save();
        }
        ImGuiUtils.Tooltip("Запускать программу при старте винды");

        ImGui.Text("Zapret Path");
        ImGui.SetNextItemWidth(-1);
        ImGui.InputText("##ZapretInput", ref configManager.Config.ZapretPath, 256);

        ImGui.Text("GoodbyeDPI Path");
        ImGui.SetNextItemWidth(-1);
        ImGui.InputText("##GoodbyeInput", ref configManager.Config.GoodbyeDpiPath, 256);

        ImGui.Text("Blockcheck Path");
        ImGui.SetNextItemWidth(-1);
        ImGui.InputText("##BlockcheckInput", ref configManager.Config.BlockcheckPath, 256);

        ImGui.Text("Background Transparency");
        ImGui.SetNextItemWidth(-1);
        ImGui.SliderFloat("##BackgroundAlpha", ref configManager.Config.BackgroundAlpha, 0.1f, 1.0f, "%.2f");
        ImGuiUtils.Tooltip("Прозрачность окна");

        ImGui.Text("Font Scale");
        ImGui.SetNextItemWidth(-1);
        ImGui.SliderFloat("##FontScale", ref configManager.Config.FontScale, 0.8f, 2.0f, "%.2f");
        ImGuiUtils.Tooltip("Размер шрифта");
    }
}