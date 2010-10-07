using System;
using System.Text;

namespace TechTalk.SpecFlow.FeatureTests.ScopedSteps
{
    [Binding]
    public class ScopedStepsBindings
    {
        private readonly StepTracker stepTracker;

        public ScopedStepsBindings(StepTracker stepTracker)
        {
            this.stepTracker = stepTracker;
        }

        [Given(@"I have a step definition (?:.*)")]
        public void GivenIHaveAStepDefinitionWithoutScope()
        {
            stepTracker.StepExecuted("Given I have a step definition without scope");
        }

        [Given(@"I have a step definition that is scoped to tag (?:.*)")]
        [StepScope(Tag = "mytag")] 
        public void GivenIHaveAStepDefinitionThatIsScopedWithMyTag()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to tag 'mytag'");
        }

        [Given(@"I have a step definition that is scoped to tag (?:.*)")]
        [StepScope(Tag = "othertag")] 
        public void GivenIHaveAStepDefinitionThatIsScopedWithOtherTag()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to tag 'othertag'");
        }

        [Given(@"I have a step definition that is scoped to feature (?:.*)")]
        [StepScope(Feature = "Scoping step definitions")] 
        public void GivenIHaveAStepDefinitionThatIsScopedToFeatureScopingStepDefinitions()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to feature 'Scoping step definitions'");
        }

        [Given(@"I have a step definition that is scoped to feature (?:.*)")]
        [StepScope(Feature = "Other feature")] 
        public void GivenIHaveAStepDefinitionThatIsScopedToFeatureOtherFeature()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to feature 'Other feature'");
        }

        [Given(@"I have a step definition that is scoped to scenario (?:.*)")]
        [StepScope(Scenario = "Scoping step definitions to scenarios")] 
        public void GivenIHaveAStepDefinitionThatIsScopedToScenarioScopingStepDefinitionsToScenarios()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to scenario 'Scoping step definitions to scenarios'");
        }

        [Given(@"I have a step definition that is scoped to scenario (?:.*)")]
        [StepScope(Scenario = "Other scenario")]
        public void GivenIHaveAStepDefinitionThatIsScopedToScenarioOtherScenario()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to scenario 'Other scenario'");
        }

        [Given(@"I have a step definition that is scoped to both tag 'mytag' and feature 'Scoping step definitions'")]
        [StepScope(Tag = "mytag", Feature = "Scoping step definitions")]
        public void GivenIHaveAStepDefinitionThatIsScopedToBothTagMytagAndFeatureScopingStepDefinitions()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to both tag 'mytag' and feature 'Scoping step definitions'");
        }

        [Given(@"I have a step definition that has two scope declaration: tag 'mytag' and feature 'Other feature'")]
        [StepScope(Tag = "mytag")]
        [StepScope(Feature = "Other feature")] 
        public void GivenIHaveAStepDefinitionThatHasTwoScopeDeclarationTagMytagAndFeatureOtherFeature()
        {
            stepTracker.StepExecuted("Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Other feature'");
        }

        [Given(@"I have a step definition that has two scope declaration: tag 'mytag' and feature 'Scoping step definitions'")]
        [StepScope(Tag = "mytag")]
        [StepScope(Feature = "Scoping step definitions")]
        public void GivenIHaveAStepDefinitionThatHasTwoScopeDeclarationTagMytagAndFeatureScopingStepDefinitions()
        {
            stepTracker.StepExecuted("Given I have a step definition that has two scope declaration: tag 'mytag' and feature 'Scoping step definitions'");
        }

        [Given(@"I have a step definition that is scoped to tag (?:.*)")]
        [StepScope(Tag = "mytag", Scenario = "More scope matches have higher precedency")]
        public void GivenIHaveAStepDefinitionThatIsScopedToTagMytagAndToFeatureScopingStepDefinitions()
        {
            stepTracker.StepExecuted("Given I have a step definition that is scoped to tag 'mytag' and to scenario 'More scope matches have higher precedency'");
        }
    }

    [Binding]
    [StepScope(Tag = "mytag")] 
    public class ScopedClassBindings
    {
        private readonly StepTracker stepTracker;

        public ScopedClassBindings(StepTracker stepTracker)
        {
            this.stepTracker = stepTracker;
        }

        [Given(@"I have a step definition that is in a class scoped to tag 'mytag'")]
        public void GivenIHaveAStepDefinitionThatIsInAClassScopedToTagMytag()
        {
            stepTracker.StepExecuted("Given I have a step definition that is in a class scoped to tag 'mytag'");
        }
    }
}
