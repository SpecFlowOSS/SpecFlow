using System.CodeDom;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public class IgnoreDecorator : ITestClassTagDecorator, ITestMethodTagDecorator
    {
        private const string IGNORE_TAG = "ignore";
        private readonly ITagFilterMatcher tagFilterMatcher;

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

        public IgnoreDecorator(ITagFilterMatcher tagFilterMatcher)
        {
            this.tagFilterMatcher = tagFilterMatcher;
        }

        private bool CanDecorateFrom(string tagName)
        {
            return tagFilterMatcher.Match(IGNORE_TAG, tagName);
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            return CanDecorateFrom(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            generationContext.UnitTestGeneratorProvider.SetTestMethodIgnore(generationContext, testMethod);
        }

        public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            return CanDecorateFrom(tagName);
        }

        public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.SetTestClassIgnore(generationContext);
        }
    }
}