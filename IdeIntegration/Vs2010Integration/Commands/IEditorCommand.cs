using EnvDTE;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public interface IEditorCommand
    {
        bool IsEnabled(Document activeDocument);
        void Invoke(Document activeDocument);
    }
}