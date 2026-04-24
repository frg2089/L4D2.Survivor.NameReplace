using System.Diagnostics.CodeAnalysis;
using System.Text;

using SteamDatabase.ValvePak;

namespace L4D2.Survivor.NameReplace;

internal sealed class Processor(SurvivorNames target)
{
    private SurvivorNames? _i18n;
    private SurvivorNames? _as;

    public static string JoinPath(params string[] paths)
        => string.Join(Package.DirectorySeparatorChar, paths);

    [MemberNotNull(nameof(_i18n))]
    [MemberNotNull(nameof(_as))]
    public bool InitSurvivorsName(string vpkPath, string lang)
    {
        _i18n ??= new();
        _as ??= new();
        return Process(vpkPath, JoinPath("resource", $"l4d360ui_{lang}.txt"), null, kvp =>
        {
            switch (kvp[0].Trim('"'))
            {
                case "L4D360UI_NamVet":
                    _i18n.Bill = kvp[1].Trim('"');
                    break;
                case "L4D360UI_TeenGirl":
                    _i18n.Zoey = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Biker":
                    _i18n.Francis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Manager":
                    _i18n.Louis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Mechanic":
                    _i18n.Ellis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Coach":
                    _i18n.Coach = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Producer":
                    _i18n.Rochelle = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Gambler":
                    _i18n.Nick = kvp[1].Trim('"');
                    break;

                case "L4D360UI_Loading_As_Zoey":
                    _as.Zoey = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Bill":
                    _as.Bill = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Francis":
                    _as.Francis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Louis":
                    _as.Louis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Coach":
                    _as.Coach = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Ellis":
                    _as.Ellis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Rochelle":
                    _as.Rochelle = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Loading_As_Nick":
                    _as.Nick = kvp[1].Trim('"');
                    break;
            }
        });
    }

    public bool Process(string vpkPath, string entryName, string outputPath, bool isClosecaption = false)
    {
        ArgumentNullException.ThrowIfNull(_i18n);
        ArgumentNullException.ThrowIfNull(_as);
        _i18n.Verify();
        _as.Verify();
        target.Verify();

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using var stream = File.Create(outputPath);
        using StreamWriter writer = new(stream, Encoding.Unicode);
        Action? action = null;
        if (isClosecaption)
        {
            action = () => writer.WriteLine($"""
                "L4D360UI_NamVet" "{target.Bill}"
                "L4D360UI_TeenGirl" "{target.Zoey}"
                "L4D360UI_Biker" "{target.Francis}"
                "L4D360UI_Manager" "{target.Louis}"
                "L4D360UI_Mechanic" "{target.Ellis}"
                "L4D360UI_Coach" "{target.Coach}"
                "L4D360UI_Producer" "{target.Rochelle}"
                "L4D360UI_Gambler" "{target.Nick}"
                "L4D360UI_Loading_As_Zoey" "{_as.Zoey.Replace(_i18n.Zoey, target.Zoey)}"
                "L4D360UI_Loading_As_Bill" "{_as.Bill.Replace(_i18n.Bill, target.Bill)}"
                "L4D360UI_Loading_As_Francis" "{_as.Francis.Replace(_i18n.Francis, target.Francis)}"
                "L4D360UI_Loading_As_Louis" "{_as.Louis.Replace(_i18n.Louis, target.Louis)}"
                "L4D360UI_Loading_As_Coach" "{_as.Coach.Replace(_i18n.Coach, target.Coach)}"
                "L4D360UI_Loading_As_Ellis" "{_as.Ellis.Replace(_i18n.Ellis, target.Ellis)}"
                "L4D360UI_Loading_As_Rochelle" "{_as.Rochelle.Replace(_i18n.Rochelle, target.Rochelle)}"
                "L4D360UI_Loading_As_Nick" "{_as.Nick.Replace(_i18n.Nick, target.Nick)}"
                """);
        }

        return Process(vpkPath, entryName, writer, kvp =>
        {
            StringBuilder txt = new(kvp[1]);
            txt.Replace(nameof(SurvivorNames.Rochelle), target.Rochelle);
            txt.Replace(_i18n.Rochelle, target.Rochelle);
            txt.Replace(nameof(SurvivorNames.Coach), target.Coach);
            txt.Replace(_i18n.Coach, target.Coach);
            txt.Replace(nameof(SurvivorNames.Ellis), target.Ellis);
            txt.Replace(_i18n.Ellis, target.Ellis);
            txt.Replace(nameof(SurvivorNames.Nick), target.Nick);
            txt.Replace(_i18n.Nick, target.Nick);
            txt.Replace(nameof(SurvivorNames.Bill), target.Bill);
            txt.Replace(_i18n.Bill, target.Bill);
            txt.Replace(nameof(SurvivorNames.Zoey), target.Zoey);
            txt.Replace(_i18n.Zoey, target.Zoey);
            txt.Replace(nameof(SurvivorNames.Francis), target.Francis);
            txt.Replace(_i18n.Francis, target.Francis);
            txt.Replace(nameof(SurvivorNames.Louis), target.Louis);
            txt.Replace(_i18n.Louis, target.Louis);

            writer.Write(kvp[0]);
            writer.Write('\t');
            writer.WriteLine(txt.ToString());
        },
        action);
    }

    private static bool Process(string vpkPath, string entryName, TextWriter? writer, Action<string[]> action, Action? onToken = null)
    {
        vpkPath = Path.GetFullPath(vpkPath);
        using Package pkg = new();
        pkg.Read(vpkPath);

        if (pkg.FindEntry(entryName) is not { } entry)
            return false;

        pkg.ReadEntry(entry, out var data);
        using StringReader reader = new(Encoding.Unicode.GetString(data));

        int brackets = 0;
        bool init = false;
        while (true)
        {
            var line = reader.ReadLine();
            if (line is null)
                break;

            if (string.IsNullOrWhiteSpace(line))
            {
                writer?.WriteLine(line);
                continue;
            }

            line = line.Trim();
            if (line.StartsWith('{'))
                brackets++;

            if (line.EndsWith('}'))
                brackets--;

            if (brackets is not 2)
            {
                writer?.WriteLine(line);
                continue;
            }

            if (!line.Contains('\t'))
            {
                writer?.WriteLine(line);
                continue;
            }

            if (brackets is 2 && !init)
            {
                init = true;
                onToken?.Invoke();
            }

            var kvp = line.Split('\t', 2, StringSplitOptions.RemoveEmptyEntries);

            action(kvp);
        }

        return brackets is 0;
    }
}
