using TechTalk.SpecFlow.Tracing.AnsiColor;

namespace TechTalk.SpecFlow.Tracing;

public interface IColorOutputTheme
{
    AnsiColor.AnsiColor Warning { get; set; }
    AnsiColor.AnsiColor Error { get; set; }
    AnsiColor.AnsiColor Done { get; set; }
    AnsiColor.AnsiColor Keyword { get; set; }
}

public class ColorOutputTheme : IColorOutputTheme
{
    public AnsiColor.AnsiColor Warning { get; set; } = AnsiColor.AnsiColor.Foreground(TerminalRgbColor.FromHex("D9B72B"));
    public AnsiColor.AnsiColor Error { get; set; } = AnsiColor.AnsiColor.Foreground(TerminalRgbColor.FromHex("FF5647"));
    public AnsiColor.AnsiColor Done { get; set; } = AnsiColor.AnsiColor.Foreground(TerminalRgbColor.FromHex("39CC8F"));
    public AnsiColor.AnsiColor Keyword { get; set; } = AnsiColor.AnsiColor.Foreground(TerminalRgbColor.FromHex("6C95EB"));
}
