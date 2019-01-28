using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class TestFileGeneratorResult
    {
        public TestFileGeneratorResult(TestGeneratorResult generatorResult, string fileName)
        {
            if (generatorResult == null)
            {
                throw new ArgumentNullException(nameof(generatorResult));
            }

            Filename = fileName ?? throw new ArgumentNullException(nameof(fileName));

            Errors = generatorResult.Errors;
            IsUpToDate = generatorResult.IsUpToDate;
            GeneratedTestCode = generatorResult.GeneratedTestCode;
        }

        /// <summary>
        /// The errors, if any.
        /// </summary>
        public IEnumerable<TestGenerationError> Errors { get; }

        /// <summary>
        /// The generated file was up-to-date.
        /// </summary>
        public bool IsUpToDate { get; }

        /// <summary>
        /// The generated test code.
        /// </summary>
        public string GeneratedTestCode { get; }

        public bool Success => Errors == null || !Errors.Any();

        public string Filename { get; }
    }
}