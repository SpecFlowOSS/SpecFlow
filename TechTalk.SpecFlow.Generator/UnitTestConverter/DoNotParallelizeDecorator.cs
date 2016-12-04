using System.Linq;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class DoNotParallelizeDecorator : ITestClassTagDecorator
    {
        private readonly string[] ignoreParallelCodeGenerationTags;
        private readonly bool generateParallelCodeForFeature;
        private readonly ITagFilterMatcher tagFilterMatcher;

        public DoNotParallelizeDecorator(ITagFilterMatcher tagFilterMatcher, GeneratorConfiguration generatorConfiguration)
        {
            this.tagFilterMatcher = tagFilterMatcher;
            this.generateParallelCodeForFeature = generatorConfiguration.GenerateParallelCodeForFeatures;
            this.ignoreParallelCodeGenerationTags = generatorConfiguration.IgnoreParallelCodeGenerationTags;
        }

        public int Priority
        { 
            get { return PriorityValues.Low; }
        }
        public bool RemoveProcessedTags
        {
            get { return true; }
        }
        public bool ApplyOtherDecoratorsForProcessedTags
        {
            get { return false; }
        }

        private bool CanDecorateFrom(string tagName)
        {
            return ignoreParallelCodeGenerationTags.Any() && generateParallelCodeForFeature &&
                   ignoreParallelCodeGenerationTags.Any(x => tagFilterMatcher.Match(x, tagName));
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            return CanDecorateFrom(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.GenerateParallelCodeForFeature = false;
        }
    }
}