using System;
using System.Runtime.Serialization;

namespace TechTalk.SpecFlow.Generator
{
    [Serializable]
    public class TestGeneratorException : Exception
    {
        public TestGenerationError[] Errors { get; private set; }

        public TestGeneratorException(TestGenerationError[] errors) : base("Unit test generation error.")
        {
            if (errors == null || errors.Length == 0)
                errors = new[] {new TestGenerationError(0, 0, "General error")};

            Errors = errors;
        }

        protected TestGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = (TestGenerationError[])info.GetValue("errors", typeof (TestGenerationError[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("errors", Errors);
        }
    }
}