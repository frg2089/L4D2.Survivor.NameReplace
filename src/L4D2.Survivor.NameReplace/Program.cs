// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;
using System.Diagnostics;

using L4D2.Survivor.NameReplace;

using Microsoft.Win32;

Console.WriteLine("Hello, World!");

string? installedPath = args.Length is 0 && OperatingSystem.IsWindows()
    ? Registry.LocalMachine
        .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 550")
        ?.GetValue("InstallLocation")
        as string
    : args[0];

string language = args.Length >= 2 ? args[1] : "schinese";

if (!Directory.Exists(installedPath))
    throw new DirectoryNotFoundException("Cannot found the Game folder.");

var config = Path.Combine(AppContext.BaseDirectory, "SurvivorNames.ini");
SurvivorNames target = new();
using (var reader = File.OpenText(config))
{
    while (await reader.ReadLineAsync() is { } line)
    {
        var kvp = line.Split('=', 2);
        if (kvp.Length is not 2)
            continue;

        switch (kvp[0])
        {
            case "Rochelle":
                target.Rochelle = kvp[1];
                break;
            case "Coach":
                target.Coach = kvp[1];
                break;
            case "Ellis":
                target.Ellis = kvp[1];
                break;
            case "Nick":
                target.Nick = kvp[1];
                break;
            case "Bill":
                target.Bill = kvp[1];
                break;
            case "Zoey":
                target.Zoey = kvp[1];
                break;
            case "Francis":
                target.Francis = kvp[1];
                break;
            case "Louis":
                target.Louis = kvp[1];
                break;
        }
    }
}

var vpk = Path.Combine(installedPath, "bin", "vpk.exe");

ImmutableArray<string> dirs = ["left4dead2", "left4dead2_dlc1", "left4dead2_dlc2", "left4dead2_dlc3"];

var vpks = dirs.Select(i => Path.Combine(installedPath, i, "pak01_dir.vpk")).ToImmutableArray();

var tmpdir = Directory.CreateTempSubdirectory();
var workdir = tmpdir.CreateSubdirectory("l4d2-survivor-name-replace");

File.WriteAllText(
    Path.Combine(workdir.FullName, "addoninfo.txt"),
    $$"""
    "AddonInfo"
    {
    	addonSteamAppID 550      
    	addonversion 1.0
    	addontitle "Survivor Name Replace"
    	addonauthor "L4D2.Survivor.NameReplace ♥ from frg2089"
    	addonDescription "{{target}}"

    	addonContent_Campaign 0
    	addonContent_Script 0
    	addonContent_Music 0
    	addonContent_Sound 0
    	addonContent_prop 0
    	addonContent_Prefab 0
    	addonContent_BackgroundMovie 0
    	addonContent_Survivor 1
    	addonContent_BossInfected 0
    	addonContent_CommonInfected 0
    	addonContent_WeaponModel 0
    	addonContent_weapon 0
    	addonContent_Skin 0
    	addonContent_Spray 0
    	addonContent_Map 0
    }
    """);

using Processor processor = new(target);

processor.InitSurvivorsName(vpks[0], language);
processor.Process(vpks[0], Processor.JoinPath("resource", $"l4d360ui_{language}.txt"), Path.Combine(workdir.FullName, "resource", $"l4d360ui_{language}.txt"));
processor.Process(vpks[0], Processor.JoinPath("resource", $"closecaption_{language}.txt"), Path.Combine(workdir.FullName, "resource", $"closecaption_{language}.txt"));
processor.Process(vpks[0], Processor.JoinPath("resource", $"subtitles_{language}.txt"), Path.Combine(workdir.FullName, "resource", $"subtitles_{language}.txt"));

Process.Start(new ProcessStartInfo()
{
    FileName = vpk,
    Arguments = workdir.FullName,
    UseShellExecute = true,
})?.WaitForExit();

Process.Start(new ProcessStartInfo()
{
    FileName = "explorer.exe",
    Arguments = $"\"{tmpdir.FullName}\"",
    UseShellExecute = true,
})?.WaitForExit();