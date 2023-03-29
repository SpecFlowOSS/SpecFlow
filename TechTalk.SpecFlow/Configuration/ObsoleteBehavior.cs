using System;

namespace TechTalk.SpecFlow.Configuration
{
#pragma warning disable CS0419
    /// <summary>
    /// Describes possibilities what to do when a test is marked with <see cref="ObsoleteAttribute"/>
    /// The default is <see cref="Warn"/>
    /// </summary>
#pragma warning restore CS0419
    public enum ObsoleteBehavior
    {
        None = 0,
        Warn = 1,
        Pending = 2,
        Error = 3
    }
}