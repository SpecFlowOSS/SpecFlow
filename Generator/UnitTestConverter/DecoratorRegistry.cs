using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestConverter
{
    public interface IDecoratorRegistry
    {
        void DecorateTestClass(TestClassGenerationContext generationContext, out List<string> unprocessedTags);
        void DecorateTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags);
    }

    public class DecoratorRegistry : IDecoratorRegistry
    {
        private readonly List<KeyValuePair<string, ITestMethodDecorator>> testClassDecorators;
        private readonly List<KeyValuePair<string, ITestMethodDecorator>> testMethodDecorators;

        private class TestMethodClassDecoratorWrapper : ITestMethodDecorator
        {
            private readonly ITestClassDecorator testClassDecorator;

            public TestMethodClassDecoratorWrapper(ITestClassDecorator testClassDecorator)
            {
                this.testClassDecorator = testClassDecorator;
            }

            public int Priority
            {
                get { return testClassDecorator.Priority; }
            }

            public bool RemoveProcessedTags
            {
                get { return testClassDecorator.RemoveProcessedTags; }
            }

            public bool ApplyOtherDecoratorsForProcessedTags
            {
                get { return testClassDecorator.ApplyOtherDecoratorsForProcessedTags; }
            }

            public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string registeredName)
            {
                return testClassDecorator.CanDecorateFrom(tagName, generationContext, registeredName);
            }

            public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string registeredName)
            {
                testClassDecorator.DecorateFrom(tagName, generationContext, registeredName);
            }
        }

        public DecoratorRegistry(IObjectContainer objectContainer)
        {
            if (objectContainer == null) throw new ArgumentNullException("objectContainer");

            testClassDecorators = objectContainer.Resolve<IDictionary<string, ITestClassDecorator>>().OrderBy(item => item.Value.Priority)
                .Select(item => new KeyValuePair<string, ITestMethodDecorator>(item.Key, new TestMethodClassDecoratorWrapper(item.Value))).ToList();
            testMethodDecorators = objectContainer.Resolve<IDictionary<string, ITestMethodDecorator>>().OrderBy(item => item.Value.Priority).ToList();
        }

        public void DecorateTestClass(TestClassGenerationContext generationContext, out List<string> unprocessedTags)
        {
            Decorate(testClassDecorators, generationContext, null, generationContext.Feature.Tags == null ? Enumerable.Empty<Tag>() : generationContext.Feature.Tags, out unprocessedTags);
        }

        public void DecorateTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            Decorate(testMethodDecorators, generationContext, testMethod, tags, out unprocessedTags);
        }

        private void Decorate(List<KeyValuePair<string, ITestMethodDecorator>> decorators, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            List<string> featureCategories = new List<string>();

            if (tags != null)
            {
                foreach (var tagName in tags.Select(t => t.Name))
                {
                    bool removeProcessedTag = false;
                    foreach (var decoratorItem in FindDecorators(decorators, tagName, generationContext, null))
                    {
                        decoratorItem.Value.DecorateFrom(tagName, generationContext, testMethod, decoratorItem.Key);
                        removeProcessedTag |= decoratorItem.Value.RemoveProcessedTags;
                        if (!decoratorItem.Value.ApplyOtherDecoratorsForProcessedTags)
                            break;
                    }

                    if (!removeProcessedTag)
                        featureCategories.Add(tagName);
                }
            }

            unprocessedTags = featureCategories;
        }

        private IEnumerable<KeyValuePair<string, ITestMethodDecorator>> FindDecorators(List<KeyValuePair<string, ITestMethodDecorator>> decorators, string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            return decorators.Where(item => item.Value.CanDecorateFrom(tagName, generationContext, testMethod, item.Key));
        }
    }
}