# L4D2.Survivor.NameReplace

求生之路2 求生者名称替换模组生成器

## 使用方法

### 编译

```bash
dotnet build
```

### 运行

```bash
dotnet run --project src/L4D2.Survivor.NameReplace/L4D2.Survivor.NameReplace.csproj
```

程序会自动检测Steam上的游戏路径，如需指定其他路径可使用：

```bash
dotnet run --project src/L4D2.Survivor.NameReplace/L4D2.Survivor.NameReplace.csproj --game-folder "D:\Steam\steamapps\common\Left 4 Dead 2"
```

### 配置

编辑 `SurvivorNames.ini` 文件来自定义求生者名称：

```ini
Rochelle = 岛风; https://steamcommunity.com/sharedfiles/filedetails/?id=626852746
Coach = 时津风; https://steamcommunity.com/sharedfiles/filedetails/?id=636907324
Ellis = 夕立; https://steamcommunity.com/sharedfiles/filedetails/?id=645180362
Nick = 天津风; https://steamcommunity.com/sharedfiles/filedetails/?id=622094252
Bill = 初月; https://steamcommunity.com/sharedfiles/filedetails/?id=1504088519
Zoey = 岛风; https://steamcommunity.com/sharedfiles/filedetails/?id=3112297048
Francis = 江风; https://steamcommunity.com/sharedfiles/filedetails/?id=839307625
Louis = 海风; https://steamcommunity.com/sharedfiles/filedetails/?id=1302754663
```

### 输出

运行后会在临时目录生成VPK模组文件，自动打开文件夹。

## 技术栈

- .NET 10.0
- DotMake.CommandLine
- ValvePak
