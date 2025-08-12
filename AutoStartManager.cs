using Microsoft.Win32;

public static class AutoStartManager
{
    private const string AppName = "VlessClientApp";

    public static void SetAutoStart(bool enable)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (enable)
            key.SetValue(AppName, Environment.ProcessPath);
        else
            key.DeleteValue(AppName, false);
    }

    public static bool IsAutoStartEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
        return key?.GetValue(AppName) != null;
    }
}