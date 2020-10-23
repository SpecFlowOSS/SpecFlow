using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace Specs.Steps
{
    [Binding]
    public class CommentBindings
    {
        private readonly ScenarioContext _scenarioContext;

        public CommentBindings(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"a comment with text (.*)")]
        public void GivenACommentWithText(string text)
        {
        }

        [Given(@"the screen height of (.*)px")]
        public void GivenTheScreenHeightOfPx(int height)
        {
        }

        [When(@"the comment box is shown")]
        public void WhenTheCommentBoxIsShown()
        {
        }

        [Then(@"the the comment size should not exceed (.*) lines")]
        public void ThenTheTheCommentSizeShouldNotExceedLines(int lines)
        {
        }
    }
}