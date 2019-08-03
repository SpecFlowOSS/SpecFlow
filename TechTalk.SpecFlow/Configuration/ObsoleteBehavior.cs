namespace TechTalk.SpecFlow.Configuration
{
    /// <summary>
    /// Describes possibilities what to do when a test is marked with <see cref="System.ObsoleteAttribute"/>
    /// The default is <see cref="Warn"/> 
    /// </summary>
    public enum ObsoleteBehavior
    {
        None = 0,
        Warn = 1,
        Pending = 2,
        Error = 3
    }
}