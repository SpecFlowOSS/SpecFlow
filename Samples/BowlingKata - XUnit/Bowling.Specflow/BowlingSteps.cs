
using Xunit;
using TechTalk.SpecFlow;

namespace Bowling.Specflow
{
    [Binding]
    public class BowlingSteps
    {
        private Game _game;

        [Given(@"a new bowling game")]
        public void GivenANewBowlingGame()
        {
            _game = new Game();
        }

        [When(@"all of my balls are landing in the gutter")]
        public void WhenAllOfMyBallsAreLandingInTheGutter()
        {
            for (int i = 0; i < 20; i++)
            {
                _game.Roll(0);
            }
        }

        [When(@"all of my rolls are strikes")]
        public void WhenAllOfMyRollsAreStrikes()
        {
            for (int i = 0; i < 12; i++)
            {
                _game.Roll(10);
            }
        }

        [Then(@"my total score should be (\d+)")]
        public void ThenMyTotalScoreShouldBe(int score)
        {
            Assert.Equal(score, _game.Score);
        }

        [When(@"I roll (\d+)")]
        public void WhenIRoll(int pins)
        {
            _game.Roll(pins);
        }

        [When(@"I roll (\d+) and (\d+)")]
        public void WhenIRoll(int pins1, int pins2)
        {
            _game.Roll(pins1);
            _game.Roll(pins2);
        }

//        [When(@"(\d+) times I roll (\d+) and (\d+)")]
//        public void WhenIRollSeveralTimes(int rollCount, int pins1, int pins2)
//        {
//            for (int i = 0; i < rollCount; i++)
//            {
//                _game.Roll(pins1);
//                _game.Roll(pins2);
//            }
//        }

        [When(@"I roll (\d+) times (\d+) and (\d+)")]
        public void WhenIRollSeveralTimes2(int rollCount, int pins1, int pins2)
        {
            for (int i = 0; i < rollCount; i++)
            {
                _game.Roll(pins1);
                _game.Roll(pins2);
            }
        }

        [When(@"I roll the following series:(.*)")]
        public void WhenIRollTheFollowingSeries(string series)
        {
            foreach (var roll in series.Trim().Split(','))
            {
                _game.Roll(int.Parse(roll));
            }
        }

        [When(@"I roll")]
        public void WhenIRoll(Table rolls)
        {
            foreach (var row in rolls.Rows)
            {
                _game.Roll(int.Parse(row["Pins"]));
            }
        }
    }
}
