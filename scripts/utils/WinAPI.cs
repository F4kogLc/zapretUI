using System.Runtime.InteropServices;

internal static class WinAPI
{
    static readonly IntPtr HWND_TOPMOST = new(-1);
    static readonly IntPtr HWND_NOTOPMOST = new(-2);
    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public static void SetTopWindow(bool state)
    {
        if (state)
            SetWindowPos(Utils.GetProcessHandle(), HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        else
            SetWindowPos(Utils.GetProcessHandle(), HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }

    ///////////////////////////////////

    const int SM_CXSCREEN = 0;
    const int SM_CYSCREEN = 1;

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    public static int GetScreenWidth() => GetSystemMetrics(SM_CXSCREEN);

    public static int GetScreenHeight() => GetSystemMetrics(SM_CYSCREEN);
}
