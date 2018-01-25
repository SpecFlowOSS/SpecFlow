using SpecFlow.TestProjectGenerator.Inputs;

namespace SpecFlow.TestProjectGenerator.ProgramLanguageDrivers
{
    public class CSharpProgramLanguageInputProjectDriver : ProgramLanguageInputProjectDriver
    {
        public override string GetBindingCode(string eventType, string code)
        {
            var staticKeyword = IsStaticEvent(eventType) ? "static" : "";
            return string.Format(@"[{0}] {1} public void {0}() 
                                {{
                                    Console.WriteLine(""BindingExecuted:{0}"");
                                    {2}
                                }}", 
                                eventType, 
                                staticKeyword, 
                                code);
        }

        public override string GetProjectFileName(string projectName)
        {
            return $"{projectName}.csproj";
        }

        public override BindingClassInput GetDefaultBindingClass()
        {
            return new BindingClassInput("DefaultBindings.cs");
        }
    }
}