namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class ParrallelizeDecorator : ITestClassTagDecorator
    {
        private const string PARRALLELIZE_TAG = "parrallelize";
        private readonly ITagFilterMatcher tagFilterMatcher;

        public ParrallelizeDecorator(ITagFilterMatcher tagFilterMatcher)
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
            return tagFilterMatcher.Match(PARRALLELIZE_TAG, tagName);
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            return CanDecorateFrom(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.SetTestClassParrallelize(generationContext);
        }
    }
}