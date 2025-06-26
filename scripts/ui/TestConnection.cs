using ImGuiNET;
using System.Numerics;

internal class TestConnection : IElement
{
    readonly ConfigManager configManager;
    readonly ProcessLauncher processLauncher;

    int currentArgumentIndex = 0;
    int successfuls = 0;
    int fails = 0;
    bool isTestingInProgress = false;
    string currentStatus = "";

    public TestConnection(ConfigManager configManager, ProcessLauncher processLauncher)
    {
        this.processLauncher = processLauncher;
        this.configManager = configManager;
    }

    public void Render()
    {
        ImGui.Text("Target");
        ImGui.InputText("##TargetInput", ref configManager.Config.Target, 256);

        ImGui.SameLine();

        if (!isTestingInProgress)
        {
            if (ImGui.Button("Run Test"))
            {
                if (!Utils.IsRunAsAdmin())
                {
                    Utils.RestartAsAdmin();
                    return;
                }

                _ = RunAllTestsAsync();
            }
        }
        else
        {
            if (ImGui.Button("Stop Test"))
            {
                StopTest();
            }
        }

        ImGui.SameLine();

        if (ImGui.Button("Run Blockcheck"))
        {
            if (!Utils.IsRunAsAdmin())
            {
                Utils.RestartAsAdmin();
                return;
            }

            Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");

            processLauncher.RunBlockcheck();
        }
        ImGuiUtils.Tooltip("После закрытия окна с блокчеком - успешные аргументы будут скопированы во вкладку Console\n\nAfter closing the blockcheck - successful arguments will be copied to the Console tab");

        ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

        ImGui.Text($"Status: ");

        ImGui.SameLine();
        ImGui.Text("(");

        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0f, 1f, 0f, 1f));
        ImGui.Text(successfuls.ToString());
        ImGui.PopStyleColor();

        ImGui.SameLine();

        ImGui.Text("/");

        ImGui.SameLine();

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
        ImGui.Text(fails.ToString());
        ImGui.PopStyleColor();

        ImGui.SameLine();

        ImGui.Text($")   (   {currentArgumentIndex}  /  {configManager.Config.TestArguments?.Count ?? 0}   )");

        ImGui.Text(currentStatus);

        ImGui.PopTextWrapPos();
    }

    async Task RunAllTestsAsync()
    {
        isTestingInProgress = true;
        ResetCounters();

        try
        {
            for (currentArgumentIndex = 0; currentArgumentIndex < configManager.Config.TestArguments.Count; currentArgumentIndex++)
            {
                if (!isTestingInProgress) break;

                var currentArgument = configManager.Config.TestArguments[currentArgumentIndex];
                currentStatus = $"{currentArgument}";

                var testResult = await TestNetwork.TestConnection(configManager.Config.Target);

                if (testResult)
                {
                    successfuls++;
                    Console.WriteLine(currentArgument);
                }
                else
                {
                    fails++;
                }

                Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");
                _ = Task.Delay(70);
                processLauncher.RunZapretTest(currentArgument);
                _ = Task.Delay(70);
            }
        }
        catch (Exception ex)
        {
            currentStatus = $"Test error: {ex.Message}";
        }
        finally
        {
            StopTest();
        }
    }

    void StopTest()
    {
        isTestingInProgress = false;
        Utils.KillProcess("winws", "goodbyedpi", "WinDivert64", "WinDivert");
    }

    void ResetCounters()
    {
        currentArgumentIndex = 0;
        successfuls = 0;
        fails = 0;
    }
}
