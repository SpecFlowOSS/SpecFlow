namespace TechTalk.SpecFlow.Configuration
{

#pragma warning disable CS0419 // Ambiguous reference in cref attribute
    /// <summary>
    /// Describes possibilities what to do when a test is marked with <see cref="System.ObsoleteAttribute"/>
    /// The default is <see cref="Warn"/> 
    /// </summary>
    //TODO: ask Andi
    public enum ObsoleteBehavior
#pragma warning restore CS0419 // Ambiguous reference in cref attribute
    {
        None = 0,
        Warn = 1,
        Pending = 2,
        Error = 3
    }
}