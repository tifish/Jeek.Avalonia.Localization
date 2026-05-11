#!/usr/bin/env dotnet
#:project DotNetRunCmd/DotNetRunCmd.csproj
using System.Diagnostics;
using static DotNetRun.Cmd;

try
{
    Run("dotnet", "nuget remove source local-test");
    return 0;
}
catch (Exception ex)
{
    EchoError(ex.Message);
    if (!Debugger.IsAttached)
        Pause("Press any key to exit...");
    return 1;
}
