using System;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public abstract class TagFilteredFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        protected readonly ITagFilterMatcher tagFilterMatcher;

        public virtual int Priority { get { return PriorityValues.Low; } }

        protected TagFilteredFeatureGeneratorProvider(ITagFilterMatcher tagFilterMatcher)
        {
            this.tagFilterMatcher = tagFilterMatcher;
        }

        protected TagFilteredFeatureGeneratorProvider() : this(new TagFilterMatcher())
        {
        }

        public bool CanGenerate(Feature feature, string registeredName)
        {
            return tagFilterMatcher.MatchPrefix(registeredName, feature);
        }

        public abstract IFeatureGenerator CreateGenerator(Feature feature, string registeredName);
    }
}