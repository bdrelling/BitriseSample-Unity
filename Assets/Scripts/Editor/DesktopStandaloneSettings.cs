using System;
using UnityEditor;

public static class DesktopStandaloneSettings
{
    private static readonly string CopyPDBFilesSettingName = "CopyPDBFiles";
    private static readonly string ScriptDebuggingSettingName = "ScriptDebugging";
    private static readonly string WaitForManagedDebuggerSettingName = "WaitForManagedDebugger";

    internal static string PlatformName => "Standalone";

    internal static bool CopyPDBFiles
    {
        get => EditorUserBuildSettings.GetPlatformSettings(PlatformName, CopyPDBFilesSettingName).ToLower() == "true";
        set => EditorUserBuildSettings.SetPlatformSettings(PlatformName, CopyPDBFilesSettingName, value.ToString().ToLower());
    }

    internal static bool ScriptDebugging
    {
        get => EditorUserBuildSettings.GetPlatformSettings(PlatformName, ScriptDebuggingSettingName).ToLower() == "true";
        set => EditorUserBuildSettings.SetPlatformSettings(PlatformName, ScriptDebuggingSettingName, value.ToString().ToLower());
    }

    internal static bool WaitForManagedDebugger
    {
        get => EditorUserBuildSettings.GetPlatformSettings(PlatformName, WaitForManagedDebuggerSettingName).ToLower() == "true";
        set => EditorUserBuildSettings.SetPlatformSettings(PlatformName, WaitForManagedDebuggerSettingName, value.ToString().ToLower());
    }
}