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
        private readonly List<ITestMethodDecorator> testClassDecorators;
        private readonly List<ITestMethodDecorator> testMethodDecorators;

        private readonly List<ITestMethodTagDecorator> testClassTagDecorators;
        private readonly List<ITestMethodTagDecorator> testMethodTagDecorators;

        #region Decorator wrappers
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

            public bool CanDecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            {
                return testClassDecorator.CanDecorateFrom(generationContext);
            }

            public void DecorateFrom(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
            {
                testClassDecorator.DecorateFrom(generationContext);
            }
        }
        private class TestMethodClassTagDecoratorWrapper : ITestMethodTagDecorator
        {
            private readonly ITestClassTagDecorator testClassTagDecorator;

            public TestMethodClassTagDecoratorWrapper(ITestClassTagDecorator testClassTagDecorator)
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
        #endregion

        public DecoratorRegistry(IObjectContainer objectContainer)
        {
            if (objectContainer == null) throw new ArgumentNullException("objectContainer");

            testClassDecorators = ResolveDecorators<ITestClassDecorator>(objectContainer, d => new TestMethodClassDecoratorWrapper(d));
            testMethodDecorators = ResolveDecorators<ITestMethodDecorator>(objectContainer, d => d);

            testClassTagDecorators = ResolveTagDecorators<ITestClassTagDecorator>(objectContainer, d => new TestMethodClassTagDecoratorWrapper(d));
            testMethodTagDecorators = ResolveTagDecorators<ITestMethodTagDecorator>(objectContainer, d => d);
        }

        private List<ITestMethodDecorator> ResolveDecorators<TDecorator>(IObjectContainer objectContainer, Func<TDecorator, ITestMethodDecorator> selector)
        {
            return objectContainer.Resolve<IDictionary<string, TDecorator>>().Select(item => selector(item.Value)).OrderBy(d => d.Priority).ToList();
        }

        private List<ITestMethodTagDecorator> ResolveTagDecorators<TDecorator>(IObjectContainer objectContainer, Func<TDecorator, ITestMethodTagDecorator> selector)
        {
            return objectContainer.Resolve<IDictionary<string, TDecorator>>().Select(item => selector(item.Value)).OrderBy(d => d.Priority).ToList();
        }

        public void DecorateTestClass(TestClassGenerationContext generationContext, out List<string> unprocessedTags)
        {
            Decorate(testClassDecorators, testClassTagDecorators, generationContext, null, generationContext.Feature.Tags == null ? Enumerable.Empty<Tag>() : generationContext.Feature.Tags, out unprocessedTags);
        }

        public void DecorateTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            Decorate(testMethodDecorators, testMethodTagDecorators, generationContext, testMethod, tags, out unprocessedTags);
        }

        private void Decorate(List<ITestMethodDecorator> decorators, List<ITestMethodTagDecorator> tagDecorators, TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<Tag> tags, out List<string> unprocessedTags)
        {
            List<string> featureCategories = new List<string>();

            foreach (var decorator in FindDecorators(decorators, generationContext, testMethod))
            {
                decorator.DecorateFrom(generationContext, testMethod);
            }

            if (tags != null)
            {
                foreach (var tagName in tags.Select(t => t.Name))
                {
                    bool removeProcessedTag = false;
                    foreach (var decorator in FindDecorators(tagDecorators, tagName, generationContext, testMethod))
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

        private IEnumerable<ITestMethodDecorator> FindDecorators(List<ITestMethodDecorator> decorators, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            return decorators.Where(decorator => decorator.CanDecorateFrom(generationContext, testMethod));
        }

        private IEnumerable<ITestMethodTagDecorator> FindDecorators(List<ITestMethodTagDecorator> decorators, string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            return decorators.Where(decorator => decorator.CanDecorateFrom(tagName, generationContext, testMethod));
        }
    }
}