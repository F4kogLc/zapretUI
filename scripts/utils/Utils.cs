using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;

internal static class Utils
{
    public static string GetAppPath()
        => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

    public static IntPtr GetProcessHandle()
        => Process.GetCurrentProcess().MainWindowHandle;

    public static void OpenURL(string url)
        => Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });

    public static bool IsProcessRunning(string processName)
        => Process.GetProcessesByName(processName).Length > 0;

    public static void KillProcess(string processName)
        => KillProcess(new[] { processName });

    public static void KillProcess(params string[] processNames)
    {
        foreach (var process in processNames.SelectMany(name => Process.GetProcessesByName(name)))
        {
            try
            {
                process.Kill();
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine($"KillProcess error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"{process.ProcessName} already killed: {ex.Message}");
            }
            finally
            {
                process.Dispose();
            }
        }
    }

    public static bool IsRunAsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void RestartAsAdmin()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = Process.GetCurrentProcess().MainModule.FileName,
            UseShellExecute = true,
            Verb = "runas"
        };

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Environment.Exit(0);
    }
}