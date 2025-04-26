using System.Diagnostics;
using System.Text;

internal class ProcessLauncher
{
    readonly ConfigManager configManager;

    public ProcessLauncher(ConfigManager configManager)
    {
        this.configManager = configManager;
    }

    public void RunAntiZapret()
        => RunProcess(configManager.Config.ZapretPath, BuildArgumentsString());

    public void RunGoodbyeDPI()
        => RunProcess(configManager.Config.GoodbyeDpiPath, BuildArgumentsString());

    public void RunBlockcheck()
        => RunProcess(configManager.Config.BlockcheckPath, "");

    string BuildArgumentsString()
    {
        var arguments = new StringBuilder();

        foreach (var feature in configManager.Config.Features.Where(f => f.IsEnabled))
        {
            foreach (var arg in feature.Arguments)
            {
                arguments.Append(arg);

                if (!arg.EndsWith(" "))
                    arguments.Append(' ');
            }
        }

        foreach (var arg in configManager.Config.SelectedArgumentsChain)
        {
            arguments.Append(arg);

            if (!arg.EndsWith(" "))
                arguments.Append(' ');
        }

        return arguments.ToString();
    }

    void RunProcess(string fileName, string arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = configManager.Config.ShowConsole ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                UseShellExecute = false,
            };

            using var process = new Process { StartInfo = startInfo };
            process.Start();

            if (configManager.Config.AutoQuit && fileName != configManager.Config.BlockcheckPath)
                Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
