using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.Specs.Silverlight4.StepDefinitions
{
    [Binding]
    public class AsyncStepDefinitions : AsyncSteps
    {
        public bool AsyncProcessStarted;
        public bool AsyncProcessComplete;

        [When(@"I initiate an asynchronous process")]
        public void When_I_initiate_an_asynchronous_process()
        {
            // NOTE: This test has knowledge that the silverlight unit testing framework is serial on the Dispatcher thread
            var thread = new Thread(() =>
                                    {
                                        // There is a small race condition with setting this here, and not waiting until it's set
                                        AsyncProcessStarted = true;
                                        Thread.Sleep(2000);

                                        Deployment.Current.Dispatcher.BeginInvoke(() => AsyncProcessComplete = true);
                                    });
            thread.Start();
        }

        [When(@"it has not yet completed")]
        public void When_it_has_not_yet_completed()
        {
            Assert.IsFalse(AsyncProcessComplete);
        }

        [When(@"I wait for it")]
        public void When_I_wait_for_it()
        {
            AsyncContext.Current.EnqueueConditional(() => AsyncProcessComplete);
        }

        [When(@"I sleep for (\d+) seconds")]
        public void When_I_sleep_for_d_seconds(int seconds)
        {
            AsyncContext.Current.EnqueueDelay(TimeSpan.FromSeconds(seconds));
        }

        [Then(@"it has completed")]
        public void Then_it_has_completed()
        {
            Assert.IsTrue(AsyncProcessComplete);
        }

        public DateTime StepStarted;
        public DateTime ActionStarted;

        [When(@"I sleep before doing executing an action")]
        public void When_I_sleep_before_doing_executing_an_action()
        {
            StepStarted = DateTime.UtcNow;
            AsyncContext.Current.EnqueueDelay(TimeSpan.FromMilliseconds(2500));
            AsyncContext.Current.EnqueueCallback(() => ActionStarted = DateTime.UtcNow);
        }

        [Then(@"the action did not start until after the delay")]
        public void Then_the_action_did_not_start_until_after_the_delay()
        {
            Assert.IsTrue((ActionStarted - StepStarted).TotalSeconds > 2, "Expected action to have started approx. 2 seconds after the step");
        }

        [When(@"I call a step and pass value '(\d+)'")]
        public void When_I_call_a_step_and_pass_value(int value)
        {
            IList<int> list;
            if (!ScenarioContext.Current.TryGetValue("stepvaluelist", out list))
            {
                list = new List<int>();
                ScenarioContext.Current["stepvaluelist"] = list;
            }

            list.Add(value);
        }

        [When(@"I call the step again (\d+) times from the step definition, passing in an increasing value starting with '(\d+)'")]
        public void When_I_call_the_step_again_d_times_from_the_step_definition_passing_in_an_increasing_value_starting_with_d(int repeat, int startValue)
        {
            for (int i = 0; i < repeat; i++)
            {
                When(string.Format("I call a step and pass value '{0}'", startValue++));
            }
        }

        [Then(@"the values passed to the steps should all be in order")]
        public void Then_the_values_passed_to_the_steps_should_all_be_in_order()
        {
            var list = ScenarioContext.Current.Get<IList<int>>("stepvaluelist");
            var orderedList = list.OrderBy(x => x);

            CollectionAssert.AreEqual(orderedList.ToArray(), list.ToArray());
        }

        [When(@"I call a step from a step definition it is not exectued until after this step")]
        public void When_I_call_a_step_from_a_step_definition_it_is_not_exectued_until_after_this_step()
        {
            When("I initiate an asynchronous process");
            Assert.IsFalse(AsyncProcessStarted, "Expected async process not to have started yet");
        }

        [When(@"it is executed before this step")]
        public void When_it_is_executed_before_this_step()
        {
            Assert.IsTrue(AsyncProcessStarted, "Expected async process to have started");
        }
    }
}