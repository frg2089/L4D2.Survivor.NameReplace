using System.Diagnostics.CodeAnalysis;
using System.Text;

using SteamDatabase.ValvePak;

namespace L4D2.Survivor.NameReplace;

internal sealed class Processor(SurvivorNames target) : IDisposable
{
    private bool _disposedValue;
    private SurvivorNames? _origin;

    private readonly Dictionary<string, Package> _packages = [];

    public static string JoinPath(params string[] paths)
        => string.Join(Package.DirectorySeparatorChar, paths);

    [MemberNotNull(nameof(_origin))]
    public bool InitSurvivorsName(string vpkPath, string lang)
    {
        _origin ??= new();
        return Process(vpkPath, JoinPath("resource", $"l4d360ui_{lang}.txt"), null, kvp =>
        {
            switch (kvp[0].Trim('"'))
            {
                case "L4D360UI_NamVet":
                    _origin.Bill = kvp[1].Trim('"');
                    break;
                case "L4D360UI_TeenGirl":
                    _origin.Zoey = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Biker":
                    _origin.Francis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Manager":
                    _origin.Louis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Mechanic":
                    _origin.Ellis = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Coach":
                    _origin.Coach = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Producer":
                    _origin.Rochelle = kvp[1].Trim('"');
                    break;
                case "L4D360UI_Gambler":
                    _origin.Nick = kvp[1].Trim('"');
                    break;
            }
        });
    }

    public bool Process(string vpkPath, string entryName, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(_origin);
        _origin.Verify();
        target.Verify();

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using var stream = File.Create(outputPath);
        using StreamWriter writer = new(stream, Encoding.Unicode);
        return Process(vpkPath, entryName, writer, kvp =>
        {
            StringBuilder txt = new(kvp[1]);
            txt.Replace(_origin.Rochelle, target.Rochelle);
            txt.Replace(_origin.Coach, target.Coach);
            txt.Replace(_origin.Ellis, target.Ellis);
            txt.Replace(_origin.Nick, target.Nick);
            txt.Replace(_origin.Bill, target.Bill);
            txt.Replace(_origin.Zoey, target.Zoey);
            txt.Replace(_origin.Francis, target.Francis);
            txt.Replace(_origin.Louis, target.Louis);

            writer.Write(kvp[0]);
            writer.Write('\t');
            writer.WriteLine(txt.ToString());
        });
    }

    private bool Process(string vpkPath, string entryName, TextWriter? writer, Action<string[]> action)
    {
        vpkPath = Path.GetFullPath(vpkPath);
        if (!_packages.TryGetValue(vpkPath, out var pkg))
        {
            pkg = new();
            pkg.Read(vpkPath);
            _packages[vpkPath] = pkg;
        }

        if (pkg.FindEntry(entryName) is not { } entry)
            return false;

        pkg.ReadEntry(entry, out var data);
        using StringReader reader = new(Encoding.Unicode.GetString(data));

        int brackets = 0;
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

            var kvp = line.Split('\t', 2, StringSplitOptions.RemoveEmptyEntries);

            action(kvp);
        }

        return brackets is 0;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                foreach (var item in _packages.Values)
                {
                    item.Dispose();
                }
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            _disposedValue = true;
        }
    }

    // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
    // ~Processor()
    // {
    //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
