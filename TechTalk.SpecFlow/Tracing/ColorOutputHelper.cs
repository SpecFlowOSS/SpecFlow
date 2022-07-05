using System;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Tracing;

public interface IColorOutputHelper
{
    bool EmitAnsiColorCodes { get; }

    string Colorize(string text, AnsiColor.AnsiColor color);
}

public class ColorOutputHelper : IColorOutputHelper
{
    private readonly SpecFlowConfiguration _specFlowConfiguration;
    private readonly bool _forceNoColor;

    public bool EmitAnsiColorCodes => _specFlowConfiguration.ColoredOutput && !_forceNoColor;

    public ColorOutputHelper(SpecFlowConfiguration specFlowConfiguration)
    {
        _specFlowConfiguration = specFlowConfiguration;
        _forceNoColor = Environment.GetEnvironmentVariable("NO_COLOR") is not null;
    }

    public string Colorize(string text, AnsiColor.AnsiColor color)
    {
        return !EmitAnsiColorCodes ? text : color.Colorize(text);
    }
}
