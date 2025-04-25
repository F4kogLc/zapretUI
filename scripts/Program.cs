using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;

internal class Program : Overlay
{
    readonly ConfigManager configManager;
    readonly TabSystem tabSystem;

    Program() : base(1920, 1080)
    {
        configManager = new ConfigManager();

        tabSystem = new TabSystem(
            new SettingsTab(configManager),
            new EditorTab(configManager),
            new HelpTab(),
            new ExitTab()
        );
    }

    static void Main(string[] args)
    {
        Program program = new();
        program.Start().Wait();
    }

    protected override Task PostInitialized()
    {
        ReplaceFont(Consts.FONT_PATH, 18, FontGlyphRangeType.Cyrillic);
        this.VSync = false;

        return Task.CompletedTask;
    }

    protected override void Render()
    {
        SetWindowStyle();
        SetWindowPos();

        ImGui.Begin($"Anti Zapret - {Consts.VERSION}");

        tabSystem.Render();

        ResetWindowStyle();

        ImGui.End();
    }

    void SetWindowPos()
    {
        var w = ImGui.GetWindowWidth();
        var h = ImGui.GetWindowHeight();

        ImGui.SetNextWindowPos(new Vector2(w, h), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize(new Vector2(960, 645), ImGuiCond.FirstUseEver);
    }

    void ResetWindowStyle()
    {
        ImGui.PopStyleColor(36);
        ImGui.PopStyleVar(3);
    }

    void SetWindowStyle()
    {
        ImGui.GetIO().FontGlobalScale = configManager.Config.FontScale;

        // Push 3 StyleVars
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 12f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(8, 8));

        // Push 36 StyleColors
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.00f, 1.00f, 1.00f, 1.00f));
        ImGui.PushStyleColor(ImGuiCol.TextDisabled, new Vector4(0.95f, 0f, 1f, 1f));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.12f, 0.12f, 0.12f, configManager.Config.BackgroundAlpha));
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.37f, 0.37f, 0.37f, 0.60f));
        ImGui.PushStyleColor(ImGuiCol.PopupBg, new Vector4(0.12f, 0.12f, 0.12f, 0.90f));
        ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.24f, 0.14f, 0.14f, 0.40f));
        ImGui.PushStyleColor(ImGuiCol.BorderShadow, new Vector4(0f, 0f, 0f, 0f));

        ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.51f, 0.51f, 0.51f, 0.30f));
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.49f, 0.49f, 0.49f, 0.4f));

        ImGui.PushStyleColor(ImGuiCol.TitleBg, new Vector4(0.38f, 0.38f, 0.38f, 1f));
        ImGui.PushStyleColor(ImGuiCol.TitleBgActive, new Vector4(0.33f, 0.02f, 0.36f, 1f));
        ImGui.PushStyleColor(ImGuiCol.TitleBgCollapsed, new Vector4(0f, 0f, 0f, 0.2f));

        ImGui.PushStyleColor(ImGuiCol.MenuBarBg, new Vector4(0.31f, 0f, 0.39f, 0.8f));

        ImGui.PushStyleColor(ImGuiCol.ScrollbarBg, new Vector4(0.11f, 0.24f, 0.48f, 0.1f));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrab, new Vector4(0.4f, 0.4f, 0.8f, 0.3f));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabActive, new Vector4(0.4f, 0.4f, 0.8f, 0.4f));
        ImGui.PushStyleColor(ImGuiCol.ScrollbarGrabHovered, new Vector4(0.78f, 0f, 1f, 1f));

        ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(0.46f, 1f, 0f, 1f));

        ImGui.PushStyleColor(ImGuiCol.SliderGrab, new Vector4(0.16f, 0.16f, 0.16f, 1f));
        ImGui.PushStyleColor(ImGuiCol.SliderGrabActive, new Vector4(0.17f, 0.16f, 0.16f, 1f));

        ImGui.PushStyleColor(ImGuiCol.Tab, new Vector4(0.38f, 0.38f, 0.38f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new Vector4(0.95f, 0f, 1f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new Vector4(0.33f, 0.02f, 0.36f, 1f));

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.37f, 0.37f, 0.37f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.37f, 0.37f, 0.37f, 1f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0f, 0f, 0f, 1f));

        ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.48f, 0.48f, 0.48f, 0.45f));
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new Vector4(0.02f, 0.02f, 0.02f, 0.8f));
        ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0f, 0f, 0f, 0.8f));

        ImGui.PushStyleColor(ImGuiCol.Separator, new Vector4(0.95f, 0f, 1f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.SeparatorActive, new Vector4(1f, 0f, 0f, 1f));
        ImGui.PushStyleColor(ImGuiCol.SeparatorHovered, new Vector4(1f, 0f, 0f, 1f));

        ImGui.PushStyleColor(ImGuiCol.ResizeGrip, new Vector4(0.49f, 0.51f, 0.53f, 0.3f));
        ImGui.PushStyleColor(ImGuiCol.ResizeGripActive, new Vector4(1f, 1f, 1f, 0.6f));
        ImGui.PushStyleColor(ImGuiCol.ResizeGripHovered, new Vector4(0f, 0f, 0f, 0.9f));
    }
}
