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
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = Consts.ZAPRET_PATH,
            Arguments = arguments.ToString(),
            WindowStyle = configManager.Config.ShowConsole ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
            //UseShellExecute = false,
            //CreateNoWindow = true
        };

        try
        {
            using Process process = new();
            process.StartInfo = startInfo;
            process.Start();

            if (configManager.Config.AutoQuit)
                Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RunBlockcheck()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Consts.BLOCKCHECK_PATH,
            UseShellExecute = false,
        };

        try
        {
            using Process process = new();
            process.StartInfo = startInfo;
            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
