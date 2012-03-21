using System;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public abstract class TagFilteredFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        public virtual int Priority { get { return 1000; } }

        public bool CanGenerate(Feature feature, string registeredName)
        {
            string expectedTagName = registeredName.StartsWith("@") ? registeredName.Substring(1) : registeredName;

            return feature.Tags != null &&
                   feature.Tags.Any(t => expectedTagName.Equals(t.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public abstract IFeatureGenerator CreateGenerator(Feature feature);
    }
}