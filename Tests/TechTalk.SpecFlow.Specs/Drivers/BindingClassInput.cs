using System.Collections.Generic;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class BindingClassInput : FileInput
    {
        public string RawClass { get; private set; }

        public List<StepBindingInput> StepBindings { get; private set; }
        public List<string> OtherBindings { get; private set; }

        public BindingClassInput(string fileName, string folder = ".")
            : base(fileName, folder)
        {
            StepBindings = new List<StepBindingInput>();
            OtherBindings = new List<string>();
        }

        public BindingClassInput(string fileName, string rawClass, string folder) : base(fileName, folder)
        {
            RawClass = rawClass;
        }
    }
}