using System.Collections.Immutable;
using System.Diagnostics;

using DotMake.CommandLine;

using Microsoft.Win32;

namespace L4D2.Survivor.NameReplace;

[CliCommand(
    Name = "L4D2.Survivor.NameReplace",
    Description = "Left 4 dead 2 求生者名称替换模组生成器")]
internal sealed class RootCommand
{
    [CliOption(Description = "游戏文件夹")]
    public DirectoryInfo? GameFolder { get; set; } = GetGameFolder();

    [CliArgument(Description = "语言")]
    public string Language { get; set; } = "schinese";

    [CliOption(Description = "求生者名称模板文件")]
    public FileInfo SurvivorNames { get; set; } = new(Path.Combine(AppContext.BaseDirectory, "SurvivorNames.ini"));

    private readonly ImmutableArray<string> _gamePaths = ["left4dead2", "left4dead2_dlc1", "left4dead2_dlc2", "left4dead2_dlc3"];

    public async Task RunAsync()
    {
        if (GameFolder is null)
            throw new DirectoryNotFoundException("Cannot found Left 4 dead 2's Game Folder.");

        var vpk = Path.Combine(GameFolder.FullName, "bin", "vpk.exe");
        var target = await LoadSurvivorNames();

        var packages = _gamePaths
            .Select(i => Path.Combine(GameFolder.FullName, i, "pak01_dir.vpk"))
            .ToImmutableArray();

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

        processor.InitSurvivorsName(packages[0], Language);
        processor.Process(packages[0], Processor.JoinPath("resource", $"l4d360ui_{Language}.txt"), Path.Combine(workdir.FullName, "resource", $"l4d360ui_{Language}.txt"));
        processor.Process(packages[0], Processor.JoinPath("resource", $"closecaption_{Language}.txt"), Path.Combine(workdir.FullName, "resource", $"closecaption_{Language}.txt"));
        processor.Process(packages[0], Processor.JoinPath("resource", $"subtitles_{Language}.txt"), Path.Combine(workdir.FullName, "resource", $"subtitles_{Language}.txt"));

        using (var process = Process.Start(new ProcessStartInfo()
        {
            FileName = vpk,
            Arguments = workdir.FullName,
            UseShellExecute = true,
        }))
            process?.WaitForExit();


        using (var process = Process.Start(new ProcessStartInfo()
        {
            FileName = "explorer.exe",
            Arguments = $"\"{tmpdir.FullName}\"",
            UseShellExecute = true,
        }))
            process?.WaitForExit();
    }

    private async Task<SurvivorNames> LoadSurvivorNames()
    {
        SurvivorNames target = new();
        using var reader = SurvivorNames.OpenText();
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

        return target;
    }

    public static DirectoryInfo? GetGameFolder()
    {
        if (!OperatingSystem.IsWindows())
            return null;

        using var key = Registry.LocalMachine
            .OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 550");

        if (key?.GetValue("InstallLocation") is not string path)
            return null;

        return new(path);
    }
}