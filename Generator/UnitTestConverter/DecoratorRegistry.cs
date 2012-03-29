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
        private readonly List<ITestMethodTagDecorator> testClassTagDecorators;
        private readonly List<ITestMethodTagDecorator> testMethodTagDecorators;

        private class TestMethodClassDecoratorWrapper : ITestMethodTagDecorator
        {
            private readonly ITestClassTagDecorator testClassTagDecorator;

            public TestMethodClassDecoratorWrapper(ITestClassTagDecorator testClassTagDecorator)
            {
                this.testClassTagDecorator = testClassTagDecorator;
            }

            public int Priority
            {
                get { return testClassTagDecorator.Priority; }
            }

            public bool RemoveProcessedTags
            {
                get { return testClassTagDecorator.RemoveProcessedTags; }
            }

            public bool ApplyOtherDecoratorsForProcessedTags
            {
                get { return testClassTagDecorator.ApplyOtherDecoratorsForProcessedTags; }
            }

            public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            {
                return testClassTagDecorator.CanDecorateFrom(tagName, generationContext);
            }

            public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            {
                testClassTagDecorator.DecorateFrom(tagName, generationContext);
            }
        }

        public DecoratorRegistry(IObjectContainer objectContainer)
        {
            if (objectContainer == null) throw new ArgumentNullException("objectContainer");

            testClassTagDecorators = ResolveDecorators<ITestClassTagDecorator>(objectContainer, d => new TestMethodClassDecoratorWrapper(d));
            testMethodTagDecorators = ResolveDecorators<ITestMethodTagDecorator>(objectContainer, d => d);
        }

        private List<ITestMethodTagDecorator> ResolveDecorators<TDecorator>(IObjectContainer objectContainer, Func<TDecorator, ITestMethodTagDecorator> selector)
        {
            return objectContainer.Resolve<IDictionary<string, TDecorator>>().Select(item => selector(item.Value)).OrderBy(d => d.Priority).ToList();
        }

        public void DecorateTestClass(TestClassGenerationContext generationContext, out List<string> unprocessedTags)
        {
            Decorate(testClassTagDecorators, generationContext, null, generationContext.Feature.Tags == null ? Enumerable.Empty<Tag>() : generationContext.Feature.Tags, out unprocessedTags);
        }

        public void DecorateTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            Decorate(testMethodTagDecorators, generationContext, testMethod, tags, out unprocessedTags);
        }

        private void Decorate(List<ITestMethodTagDecorator> decorators, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            List<string> featureCategories = new List<string>();

            if (tags != null)
            {
                foreach (var tagName in tags.Select(t => t.Name))
                {
                    bool removeProcessedTag = false;
                    foreach (var decorator in FindDecorators(decorators, tagName, generationContext, null))
                    {
                        decorator.DecorateFrom(tagName, generationContext, testMethod);
                        removeProcessedTag |= decorator.RemoveProcessedTags;
                        if (!decorator.ApplyOtherDecoratorsForProcessedTags)
                            break;
                    }

                    if (!removeProcessedTag)
                        featureCategories.Add(tagName);
                }
            }

            unprocessedTags = featureCategories;
        }

        private IEnumerable<ITestMethodTagDecorator> FindDecorators(List<ITestMethodTagDecorator> decorators, string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            return decorators.Where(decorator => decorator.CanDecorateFrom(tagName, generationContext, testMethod));
        }
    }
}