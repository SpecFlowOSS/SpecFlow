using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public class BindingSteps
    {
        private readonly ProjectsDriver _projectsDriver;

        public BindingSteps(ProjectsDriver projectsDriver)
        {
            _projectsDriver = projectsDriver;
        }

        [Given(@"all steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass()
        {
            GivenAllStepsAreBoundAndPass(ScenarioBlock.Given);
            GivenAllStepsAreBoundAndPass(ScenarioBlock.When);
            GivenAllStepsAreBoundAndPass(ScenarioBlock.Then);
        }

        [Given(@"all steps are bound and fail")]
        public void GivenAllStepsAreBoundAndFail()
        {
            GivenAllStepsAreBoundAndFail(ScenarioBlock.Given);
            GivenAllStepsAreBoundAndFail(ScenarioBlock.When);
            GivenAllStepsAreBoundAndFail(ScenarioBlock.Then);
        }

        [Given(@"all steps are bound and are pending")]
        public void GivenAllStepsAreBoundAndArePending()
        {
            GivenAllStepsAreBoundAndArePending(ScenarioBlock.Given);
            GivenAllStepsAreBoundAndArePending(ScenarioBlock.When);
            GivenAllStepsAreBoundAndArePending(ScenarioBlock.Then);
        }

        [Given(@"all '(.*)' steps are bound and fail")]
        public void GivenAllStepsAreBoundAndFail(ScenarioBlock scenarioBlock)
        {
            _projectsDriver.AddFailingStepBinding(scenarioBlock.ToString(), ".*");
        }

        [Given(@"all '(.*)' steps are bound and pass")]
        public void GivenAllStepsAreBoundAndPass(ScenarioBlock scenarioBlock)
        {
            _projectsDriver.AddStepBinding(scenarioBlock.ToString(), ".*", "//pass", "'pass");
        }

        [Given(@"all '(.*)' steps are bound and are pending")]
        public void GivenAllStepsAreBoundAndArePending(ScenarioBlock scenarioBlock)
        {
            _projectsDriver.AddStepBinding(scenarioBlock.ToString(), ".*", "ScenarioContext.Current.Pending();", "ScenarioContext.Current.Pending()");
        }

        [Given(@"the following step definition in the project '(.*)'")]
        [Given(@"the following binding class in the project '(.*)'")]
        public void GivenTheFollowingStepDefinitionInTheProject(string projectName, string stepDefinition)
        {
            _projectsDriver.AddStepBinding(projectName, stepDefinition);
        }

        [Given(@"the following hooks?")]
        [Given(@"the following step definitions?")]
        [Given(@"the following step argument transformations?")]
        [Given(@"the following binding methods?")]
        public void GivenTheFollowingBindings(string bindingCode)
        {
            _projectsDriver.AddStepBinding(bindingCode);
        }
        
        [Given(@"a '(.*)' step definition with name '(.*)'")]
        public void GivenTheFollowingNamedBindings(string attributeName, string name)
        {
            _projectsDriver.AddLoggingStepBinding(attributeName, name, "");
        }

        [Given(@"a hook '(.*)' for '(.*)' with tags? '([^']*)'")]
        public void GivenAHookForForTags(string methodName, string eventType, string tags)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, hookTypeAttributeTagsString: tags);
        }

        [Given(@"a hook '(.*)' for '(.*)' with method scopes? '([^']*)'")]
        [Given(@"a hook '(.*)' for '(.*)' with scopes? '(.*)'")]
        public void GivenAHookForForScopesOnTheHookMethod(string methodName, string eventType, string tagScopes)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, null, methodScopeAttributeTagsString: tagScopes);
        }

        [Given(@"a hook '([^']*)' for '([^']*)' with class scopes? '(.*)'")]
        public void GivenAHookForForScopesOnTheClass(string methodName, string eventType, string methodScopeTags)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, null, methodScopeAttributeTagsString: methodScopeTags);
        }

        [Given(@"a hook '([^']*)' for '([^']*)' with tags? '([^']*)' and class scopes? '(.*)'")]
        public void GivenAHookForTagsForScopesOnTheClass(string methodName, string eventType, string hookTypeTags, string methodScopeTags)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, hookTypeTags, methodScopeAttributeTagsString: methodScopeTags);
        }
        
        [Given(@"a hook '(.*)' for '(.*)' with method scopes? '(.*)' and class scopes? '(.*)'")]
        public void GivenAHookForForScopesOnTheHookMethodAndScopesOnClass(string methodName, string eventType, string methodScopeTags, string classScopeTags)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, null, methodScopeAttributeTagsString: methodScopeTags, classScopeAttributeTagsString: classScopeTags);
        }

        [Given(@"a hook '(.*)' for '(.*)' with tags? '(.*)' and method scopes? '(.*)' and class scopes? '(.*)'")]
        public void GivenAHookForForTagsAndScopesOnTheHookMethodAndScopesOnClass(string methodName, string eventType, string hookTypeTags, string methodScopeTags, string classScopeTags)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, null, methodScopeAttributeTagsString: classScopeTags);
        }

        [Given(@"a hook '(.*)' for '([^']*)'")]
        public void GivenAnEventBindingFor(string methodName, string eventType)
        {
            _projectsDriver.AddHookBinding(eventType, methodName);
        }

        [Given(@"a long running hook '(.*)' for '(.*)'")]
        public void GivenALongRunningHookFor(string methodName, string eventType)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, code: "System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(250));");
        }


        [Given(@"a hook '(.*)' for '([^']*)' with order '([^']*)'")]
        public void GivenAHookForWithOrder(string methodName, string eventType, int hookOrder)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, order: hookOrder);
        }

        [Given(@"a hook '(.*)' for '([^']*)' with order '([^']*)' throwing an exception")]
        public void GivenAHookForWithOrderThrowingAnException(string methodName, string eventType, int hookOrder)
        {
            _projectsDriver.AddHookBinding(eventType, methodName, order: hookOrder, code: $"throw new System.Exception(\"Error in Hook: {eventType}\");");
        }

        [Given(@"the following binding class")]
        [Given(@"the following class")]
        public void GivenTheFollowingBindingClass(string rawBindingClass)
        {
            _projectsDriver.AddBindingClass(rawBindingClass);
        }
    }
}