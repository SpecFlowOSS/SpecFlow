using System.Collections.Generic;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class BindingClassInput : FileInput
    {
        public List<StepBindingInput> StepBindings { get; private set; }
        public List<string> OtherBindings { get; private set; }

        public BindingClassInput(string fileName, string folder = ".")
            : base(fileName, folder)
        {
            StepBindings = new List<StepBindingInput>();
            OtherBindings = new List<string>();
        }
    }
}