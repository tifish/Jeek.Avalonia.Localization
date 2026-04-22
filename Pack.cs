#!/usr/bin/env dotnet
#:package DotNetRunCmd@2.2.1

using System.Diagnostics;
using static DotNetRun.Cmd;

try
{
    var projectName = "Jeek.Avalonia.Localization";

    Echo($"Packing {projectName}");
    var binPath = Path.Combine(projectName, "bin");
    Directory.Delete(binPath, true);
    Run("dotnet", $"build {projectName} -c Release");
    Run("dotnet", $"pack {projectName} -c Release");

    Echo($"Adding to local test NuGet source");
    var localNugetPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".nuget-local-test"
    );
    Directory.CreateDirectory(localNugetPath);
    RunIgnoreExitCode("dotnet", $"nuget add source {localNugetPath} -n local-test");
    if (OperatingSystem.IsWindows())
        Robocopy($@"{binPath}", localNugetPath, "*.nupkg");
    else
        Run("rsync", $"{binPath}/*.nupkg {localNugetPath}/");

    return 0;
}
catch (Exception ex)
{
    EchoError(ex.Message);
    if (!Debugger.IsAttached)
        Pause("Press any key to exit...");
    return 1;
}
