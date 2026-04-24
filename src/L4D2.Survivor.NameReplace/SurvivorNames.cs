using System.Diagnostics.CodeAnalysis;

namespace L4D2.Survivor.NameReplace;

internal sealed class SurvivorNames
{
    public string? Rochelle { get; set; }
    public string? Coach { get; set; }
    public string? Ellis { get; set; }
    public string? Nick { get; set; }
    public string? Bill { get; set; }
    public string? Zoey { get; set; }
    public string? Francis { get; set; }
    public string? Louis { get; set; }

    [MemberNotNull(nameof(Bill), nameof(Zoey), nameof(Francis), nameof(Louis), nameof(Ellis), nameof(Coach), nameof(Rochelle), nameof(Nick))]
    public void Verify()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Bill);
        ArgumentException.ThrowIfNullOrWhiteSpace(Zoey);
        ArgumentException.ThrowIfNullOrWhiteSpace(Francis);
        ArgumentException.ThrowIfNullOrWhiteSpace(Louis);
        ArgumentException.ThrowIfNullOrWhiteSpace(Ellis);
        ArgumentException.ThrowIfNullOrWhiteSpace(Coach);
        ArgumentException.ThrowIfNullOrWhiteSpace(Rochelle);
        ArgumentException.ThrowIfNullOrWhiteSpace(Nick);
    }

    public override string ToString() => $"""
        Rochelle => {Rochelle};
        Coach => {Coach};
        Ellis => {Ellis};
        Nick => {Nick};
        Bill => {Bill};
        Zoey => {Zoey};
        Francis => {Francis};
        Louis => {Louis};
        """;
}