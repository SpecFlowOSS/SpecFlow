namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface IFileAssociationDetector
    {
        bool? IsAssociated();
        bool SetAssociation();
    }
}