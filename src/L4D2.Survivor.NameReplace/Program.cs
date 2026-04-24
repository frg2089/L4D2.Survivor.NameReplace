using DotMake.CommandLine;

using L4D2.Survivor.NameReplace;

return args switch
{
    { Length: not 0 } => Cli.Run<RootCommand>(args),
    _ => Cli.Run<RootCommand>("--help")
};
