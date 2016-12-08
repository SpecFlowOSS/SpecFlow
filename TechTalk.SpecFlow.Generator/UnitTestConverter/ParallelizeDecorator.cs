using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class ParallelizeDecorator : ITestClassTagDecorator, ITestClassDecorator
    {
        private readonly string[] ignoreParallelCodeGenerationTags;
        private readonly ITagFilterMatcher tagFilterMatcher;
        private readonly bool generateParallelCodeForFeatures;

        public ParallelizeDecorator(ITagFilterMatcher tagFilterMatcher, GeneratorConfiguration generatorConfiguration)
        {
            this.tagFilterMatcher = tagFilterMatcher;
            this.generateParallelCodeForFeatures = generatorConfiguration.GenerateParallelCodeForFeatures;
            this.ignoreParallelCodeGenerationTags = generatorConfiguration.IgnoreParallelCodeGenerationTags;
        }

        public int Priority
        { 
            get { return PriorityValues.Low; }
        }

        public bool CanDecorateFrom(TestClassGenerationContext generationContext)
        {
            return CanGenerateParallelCodeBasedOnConfiguration(generationContext) && !IgnoreParallelCodeGenerationBasedOnTags(generationContext.Feature.Tags.Select(x=>x.GetNameWithoutAt()));
        }

        public void DecorateFrom(TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.SetTestClassParallelize(generationContext);
        }

        public bool RemoveProcessedTags
        {
            get { return true; }
        }
        public bool ApplyOtherDecoratorsForProcessedTags
        {
            get { return false; }
        }

        private bool CanGenerateParallelCodeBasedOnConfiguration(TestClassGenerationContext generationContext)
        {
            return generationContext.UnitTestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution) && generateParallelCodeForFeatures;
        }
        private bool IgnoreParallelCodeGenerationBasedOnTags(IEnumerable<string> tagName)
        {
            return ignoreParallelCodeGenerationTags.Any(x => tagFilterMatcher.Match(x, tagName));
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            return IgnoreParallelCodeGenerationBasedOnTags(new[] {tagName});
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {

        }
    }
}