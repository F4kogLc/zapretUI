using Microsoft.Win32;
using System.Diagnostics;

internal class AutoStartManager
{
    const string RegistryKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    readonly string appName;
    readonly string exePath;

    public AutoStartManager(string appName)
    {
        this.appName = appName;
        exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().MainModule.FileName);
    }

    public bool IsAutoStartEnabled()
    {
        try
        {
            var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
            return key?.GetValue(appName)?.ToString() == exePath;
        }
        catch
        {
            return false;
        }
    }

    public void SetAutoStart(bool enable)
    {
        try
        {
            var key = Registry.CurrentUser.CreateSubKey(RegistryKeyPath);

            if (enable)
            {
                key.SetValue(appName, exePath);
            }
            else
            {
                key.DeleteValue(appName, false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error autostart: {ex.Message}");
        }
    }
}
