using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class BuildUtility
{
    #region Constants

    private static readonly string BuildFolder = "builds";
    private static readonly string ProductName = Application.productName;
    // private static readonly string[] Scenes = new[] { "Assets/Scenes/Title.unity", "Assets/Scenes/Game.unity" };
    private static readonly BuildOptions DevelopmentBuildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer;

    private static string[] ActiveScenes => EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

    //private static BuildOptions ReleaseBuildOptions = BuildOptions.None;
    //options.options = BuildOptions.AutoRunPlayer;

    #endregion

    #region Properties

    // private CommandLineTools CommandLineTools { get; set; } = new CommandLineTools();

    #endregion

    #region Methods

    #region Menu Items

    [MenuItem("Build/Android/Default", priority = 0)]
    public static void BuildAndroid() => BuildForTarget(BuildTarget.Android);

    [MenuItem("Build/iOS/Default", priority = 1)]
    public static void BuildIos()
    {
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Debug;
        EditorUserBuildSettings.development = true;

        BuildForTarget(BuildTarget.iOS);
    }

    [MenuItem("Build/Linux/32 bit", priority = 2)]
    public static void BuildLinux() => BuildForTarget(BuildTarget.StandaloneLinux);

    [MenuItem("Build/Linux/64 bit", priority = 3)]
    public static void BuildLinux64() => BuildForTarget(BuildTarget.StandaloneLinux64);

    // [MenuItem("Build/Linux/Universal", priority = 4)]
    // public static void BuildLinuxUniversal() => BuildForTarget(BuildTarget.StandaloneLinuxUniversal);

    [MenuItem("Build/macOS/Default", priority = 5)]
    public static void BuildMacOs() => BuildForTarget(BuildTarget.StandaloneOSX);

    [MenuItem("Build/WebGL/Default", priority = 5)]
    public static void BuildWebGL() => BuildForTarget(BuildTarget.WebGL);

    [MenuItem("Build/Windows/32 bit", priority = 7)]
    public static void BuildWindows() => BuildForTarget(BuildTarget.StandaloneWindows);

    [MenuItem("Build/Windows/64 bit", priority = 8)]
    public static void BuildWindows64() => BuildForTarget(BuildTarget.StandaloneWindows64);

    [MenuItem("Build/Windows/UWP", priority = 9)]
    public static void BuildUniversalWindowsPlatform() => BuildForTarget(BuildTarget.WSAPlayer);

    #endregion

    #region Menu Item Validation

    [MenuItem("Build/Linux/32 bit", true)]
    [MenuItem("Build/Linux/64 bit", true)]
    [MenuItem("Build/Linux/Universal", true)]
    public static bool CanBuildLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    [MenuItem("Build/macOS/Default", true)]
    public static bool CanBuildMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    //[MenuItem("Build/Windows/32 bit", true)]
    //[MenuItem("Build/Windows/64 bit", true)]
    //public static bool CanBuildWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    #endregion

    #region Utilities

    public static void BuildForTarget(BuildTarget target, string outputPath = null)
    {
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);

        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);

        var options = new BuildPlayerOptions
        {
            scenes = ActiveScenes,
            locationPathName = outputPath ?? GetPlayerPathForTarget(target),
            options = DevelopmentBuildOptions,
            target = target,
            targetGroup = targetGroup,
        };

        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                DesktopStandaloneSettings.CopyPDBFiles = true;
                DesktopStandaloneSettings.ScriptDebugging = true;
                //DesktopStandaloneSettings.WaitForManagedDebugger = true;
                break;
        }

        BuildPipeline.BuildPlayer(options);
    }

    public static void CommandLineBuild()
    {
        var tools = new CommandLineTools();
        tools.PrintInputs();

        var target = tools.Inputs.BuildTarget;
        var outputPath = tools.Inputs.OutputPath;

        switch (target)
        {
            case BuildTarget.Android:
                EditorPrefs.SetString("AndroidSdkRoot", tools.Inputs.AndroidSdkPath);
                PlayerSettings.Android.keystoreName = tools.Inputs.AndroidKeystorePath;
                PlayerSettings.Android.keystorePass = tools.Inputs.AndroidKeystorePassword;
                PlayerSettings.Android.keyaliasName = tools.Inputs.AndroidKeystoreAlias;
                PlayerSettings.Android.keyaliasPass = tools.Inputs.AndroidKeystoreAliasPassword;
                break;
        }

        BuildForTarget(target, outputPath);
    }

    private static string GetPlayerPathForTarget(BuildTarget target) => $"{GetBuildFolderForTarget(target)}/{GetFilenameForTarget(target)}";

    private static string GetBuildFolderForTarget(BuildTarget target) => $"{BuildFolder}/{target}";

    private static string GetFilenameForTarget(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.iOS:
            case BuildTarget.WebGL:
                // these output into a directory, not a file, so no file extension is required
                return $"{ProductName}";
            default:
                return $"{ProductName}{GetFileExtension(target)}";
        }
    }

    private static string GetFileExtension(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.StandaloneOSX:
                return ".app";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.WSAPlayer:
                return ".exe";
            case BuildTarget.StandaloneLinux:
                return ".x86";
            case BuildTarget.StandaloneLinux64:
                return ".x64";
            case BuildTarget.StandaloneLinuxUniversal:
                return ".x86_64";
            case BuildTarget.iOS:
                return ".ipa";
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.WebGL:
                return ".html";
            //case BuildTarget.WebGL:
            //case BuildTarget.WSAPlayer:
            //case BuildTarget.PS4:
            //case BuildTarget.XboxOne:
            //case BuildTarget.tvOS:
            //case BuildTarget.Switch:
            //case BuildTarget.Lumin:
            //case BuildTarget.NoTarget:
            default:
                throw new InvalidOperationException($"Unable to get filename. Invalid build target '{target}'.");
        }
    }

    public class CommandLineTools
    {
        public CommandLineInputs Inputs { get; set; }
        public CommandLineLogger Logger { get; set; }

        public CommandLineTools()
        {
            this.Inputs = new CommandLineInputs();
            this.Logger = new CommandLineLogger();
        }

        public class CommandLineInputs
        {
            public string AndroidKeystoreAlias;
            public string AndroidKeystoreAliasPassword;
            public string AndroidKeystorePassword;
            public string AndroidKeystorePath;
            public string AndroidSdkPath;
            public BuildTarget BuildTarget;
            public string OutputPath;

            public CommandLineInputs()
            {
                var cmdArgs = Environment.GetCommandLineArgs();

                for (var i = 0; i < cmdArgs.Length; i++)
                {
                    var valueIndex = i + 1;
                    var value = (valueIndex < cmdArgs.Length) ? cmdArgs[valueIndex] : null;

                    if (cmdArgs[i].Equals("-target"))
                        this.BuildTarget = this.GetBuildTarget(value);
                    if (cmdArgs[i].Equals("-androidSdkPath"))
                        this.AndroidSdkPath = value;
                    if (cmdArgs[i].Equals("-output"))
                        this.OutputPath = value;
                    if (cmdArgs[i].Equals("-androidKeystorePath"))
                        this.AndroidKeystorePath = value;
                    if (cmdArgs[i].Equals("-androidKeystoreAlias"))
                        this.AndroidKeystoreAlias = value;
                    if (cmdArgs[i].Equals("-androidKeystorePassword"))
                        this.AndroidKeystorePassword = value;
                    if (cmdArgs[i].Equals("-androidKeystoreAliasPassword"))
                        this.AndroidKeystoreAliasPassword = value;
                }
            }

            public BuildTarget GetBuildTarget(string target)
            {
                switch (target.ToLower())
                {
                    case "ios":
                        return BuildTarget.iOS;
                    case "android":
                        return BuildTarget.Android;
                    default:
                        throw new InvalidOperationException("Unable to get build target. Invalid value '{target}'.");
                }
            }
        }

        public class CommandLineLogger
        {
            private bool Initialized = false;

            private void _init()
            {
                if (!this.Initialized)
                {
                    var sw = new StreamWriter(Console.OpenStandardOutput(), System.Text.Encoding.ASCII)
                    {
                        AutoFlush = true
                    };
                    Console.SetOut(sw);
                    this.Initialized = true;
                }
            }

            public void Fail(string message) { this._init(); Console.WriteLine("\x1b[31m" + message + "\x1b[0m"); }
            public void Done(string message) { this._init(); Console.WriteLine("\x1b[32m" + message + "\x1b[0m"); }
            public void Info(string message) { this._init(); Console.WriteLine("\x1b[34m" + message + "\x1b[0m"); }
            public void Warn(string message) { this._init(); Console.WriteLine("\x1b[33m" + message + "\x1b[0m"); }
            public void Print(string message) { this._init(); Console.WriteLine(message); }
        }

        public void PrintInputs()
        {
            this.Logger.Info("Bitrise Unity build script inputs:");
            this.Logger.Print(" -buildOutput: " + this.Inputs.OutputPath);
            this.Logger.Print(" -buildPlatform: " + this.Inputs.BuildTarget.ToString());
            this.Logger.Print(" -androidSdkPath: " + this.Inputs.AndroidSdkPath);
            this.Logger.Print(" -androidKeystorePath: " + this.Inputs.AndroidKeystorePath);
            this.Logger.Print(" -androidKeystoreAlias: " + (string.IsNullOrEmpty(this.Inputs.AndroidKeystoreAlias) ? "" : "***"));
            this.Logger.Print(" -androidKeystorePassword: " + (string.IsNullOrEmpty(this.Inputs.AndroidKeystorePassword) ? "" : "***"));
            this.Logger.Print(" -androidKeystoreAliasPassword: " + (string.IsNullOrEmpty(this.Inputs.AndroidKeystoreAliasPassword) ? "" : "***"));
            this.Logger.Print("");
        }
    }

    #endregion

    #endregion
}

