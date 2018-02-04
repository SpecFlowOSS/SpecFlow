using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    public abstract class ProgramLanguageInputProjectDriver : IProgramLanguageInputProjectDriver
    {
        protected bool IsStaticEvent(string eventType)
        {
            return eventType == "BeforeFeature" || eventType == "AfterFeature" || eventType == "BeforeTestRun" || eventType == "AfterTestRun";
        }

        public abstract string GetBindingCode(string eventType, string code);
        public abstract string GetProjectFileName(string projectName);
        public abstract BindingClassInput GetDefaultBindingClass();
    }

}