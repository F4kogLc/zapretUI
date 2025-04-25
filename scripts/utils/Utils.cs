using System.Diagnostics;
using System.Security.Principal;
using ImGuiNET;

internal static class Utils
{
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