#nullable enable
using System;
using System.Globalization;

namespace TechTalk.SpecFlow.Tracing.AnsiColor;

public readonly struct TerminalRgbColor
{
    public static readonly TerminalRgbColor None = new(-1, -1, -1);

    public static TerminalRgbColor From(int r, int g, int b) => new(r, g, b);

    public static TerminalRgbColor FromHex(string hex)
    {
        if (hex.StartsWith("#")) hex = hex.Substring(1);
        if (hex.Length != 6) throw new ArgumentException("Invalid hex form, should be RRGGBB", nameof(hex));
        var r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        var g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        var b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        return new TerminalRgbColor(r, g, b);
    }

    public int R { get; }

    public int G { get; }

    public int B { get; }

    public TerminalRgbColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }

    public string ToColorParameter()
    {
        return $"2;{R};{G};{B}";
    }

    public bool Equals(TerminalRgbColor other)
    {
        return R == other.R && G == other.G && B == other.B;
    }

    public override bool Equals(object? obj)
    {
        return obj is TerminalRgbColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = R;
            hashCode = (hashCode * 397) ^ G;
            hashCode = (hashCode * 397) ^ B;
            return hashCode;
        }
    }

    public static bool operator ==(TerminalRgbColor left, TerminalRgbColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TerminalRgbColor left, TerminalRgbColor right)
    {
        return !(left == right);
    }
}
