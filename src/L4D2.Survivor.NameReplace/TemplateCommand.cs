using System.Diagnostics;

using DotMake.CommandLine;

namespace L4D2.Survivor.NameReplace;

[CliCommand(Description = "创建求生者名模板", Parent = typeof(RootCommand))]
internal sealed class TemplateCommand
{
    [CliOption(Description = "求生者名称模板文件")]
    public FileInfo SurvivorNames { get; set; } = new(Path.Combine(AppContext.BaseDirectory, "SurvivorNames.ini"));

    public async Task RunAsync()
    {
        await using (var writer = SurvivorNames.CreateText())
        {
            await writer.WriteLineAsync("Rochelle = Rochelle");
            await writer.WriteLineAsync("Coach = Coach");
            await writer.WriteLineAsync("Ellis = Ellis");
            await writer.WriteLineAsync("Nick = Nick");
            await writer.WriteLineAsync("Bill = Bill");
            await writer.WriteLineAsync("Zoey = Zoey");
            await writer.WriteLineAsync("Francis = Francis");
            await writer.WriteLineAsync("Louis = Louis");
        }

        using var process = Process.Start(SurvivorNames.FullName);
        process?.WaitForExit();
    }
}
