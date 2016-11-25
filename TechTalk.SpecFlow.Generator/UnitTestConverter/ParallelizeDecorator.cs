namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class ParallelizeDecorator : ITestClassTagDecorator
    {
        private const string PARALLELIZE_TAG = "parallelize";
        private readonly ITagFilterMatcher tagFilterMatcher;

        public ParallelizeDecorator(ITagFilterMatcher tagFilterMatcher)
        {
            this.tagFilterMatcher = tagFilterMatcher;
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
            return tagFilterMatcher.Match(PARALLELIZE_TAG, tagName);
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            return CanDecorateFrom(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.SetTestClassParallelize(generationContext);
        }
    }
}