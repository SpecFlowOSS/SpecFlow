using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    public class TestGeneratorResult
    {
        public IEnumerable<TestGenerationError> Errors { get; private set; }
        public bool IsUpToDate { get; private set; }
        public string GeneratedTestCode { get; private set; }

        public bool Success { get { return Errors == null || !Errors.Any(); } }

        public TestGeneratorResult(params TestGenerationError[] errors)
            : this((IEnumerable<TestGenerationError>)errors)
        {
        }

        public TestGeneratorResult(IEnumerable<TestGenerationError> errors)
        {
            if (errors == null) throw new ArgumentNullException("errors");
            if (errors.Count() == 0) throw new ArgumentException("no errors provided", "errors");

            Errors = errors.ToArray();
        }

        public TestGeneratorResult(string generatedTestCode, bool isUpToDate)
        {
            IsUpToDate = isUpToDate;
            GeneratedTestCode = generatedTestCode;
        }
    }
}