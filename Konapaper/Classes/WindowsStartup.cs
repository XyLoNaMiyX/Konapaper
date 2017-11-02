// Made by Lonami Exo | 24-03-2016
using Microsoft.Win32;
using System;
using System.Reflection;

public static class WindowsStartup
{
    #region Public methods

    /// <summary>
    /// Determines whether the current application is running at start up or not
    /// </summary>
    /// <returns>Returns true if the application is running at startup</returns>
    public static bool IsRunningAtStartUp()
        => !string.IsNullOrEmpty((string)getRunKey(false).GetValue(getAsmNameLocation().Item1));

    /// <summary>
    /// Enables the application at windows start up
    /// </summary>
    /// <param name="args">(Optional) Application arguments</param>
    /// <returns>Returns true if the operation was successful</returns>
    public static bool EnableRunAtStartUp(string args = null)
    {
        try
        {
            var nameLoc = getAsmNameLocation();
            getRunKey(true).SetValue(nameLoc.Item1,
                string.IsNullOrEmpty(args) ? nameLoc.Item2 : nameLoc.Item2 + " " + args);

            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Disables the application at windows start up
    /// </summary>
    /// <returns>Returns true if the operation was successful</returns>
    public static bool DisableRunAtStartUp()
    {
        try
        {
            getRunKey(true).DeleteValue(getAsmNameLocation().Item1);
            return true;
        }
        catch { return false; }
    }

    #endregion

    #region Private methods

    // gets the executing assembly name and location
    static Tuple<string, string> getAsmNameLocation()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return new Tuple<string, string>(assembly.GetName().Name, "\"" + assembly.Location + "\"");
    }

    // run key path
    const string runKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    // retrieve the run registry key
    static RegistryKey getRunKey(bool writable) => Registry.CurrentUser.OpenSubKey(runKeyPath, writable);

    #endregion
}