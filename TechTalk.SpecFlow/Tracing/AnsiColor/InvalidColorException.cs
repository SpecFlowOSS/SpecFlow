#nullable enable
using System;

namespace TechTalk.SpecFlow.Tracing.AnsiColor;

public class InvalidColorException : Exception
{
    public InvalidColorException(string? message)
        : base(message)
    {
    }
}
