using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    [TestFixture]
    public class StepDefinitionSkeletonProviderTests
    {
        private IStepDefinitionSkeletonProvider skeletonProviderCS;
        private IStepDefinitionSkeletonProvider skeletonProviderVB;

        [SetUp]
        public void Init()
        {
            skeletonProviderCS = new StepDefinitionSkeletonProviderCS();
            skeletonProviderVB = new StepDefinitionSkeletonProviderVB();
        }

        #region CSharp
        [Test]
        public void Produces_CSharp_Method_Skeleton()
        {
            StringBuilder expected = new StringBuilder();
            expected.Append(
@"[Given(@""I have a new step"")]
public void GivenIHaveANewStep()
{
    ScenarioContext.Current.Pending();
}
");
            StepArgs stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, "I have a new step", null, null, null);

            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Parsed_Doubles()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Then(@""I can parse a double """"(.*)"""" and another double """"(.*)"""""")]
public void ThenICanParseADoubleAndAnotherDouble(double double1, double double2)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                                         "I can parse a double \"3.4\" and another double \"902.302\"", null, null, null);
            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Parsed_Dates()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[When(@""I can parse dates such as """"(.*)"""", """"(.*)"""" and """"(.*)"""""")]
public void WhenICanParseDatesSuchAsAnd(DateTime dateTime1, DateTime dateTime2, DateTime dateTime3)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                "I can parse dates such as \"20/10/02\", \"12:00\" and \"20.12.09\"", null, null, null);

            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Parsed_Ints()
        {
            var expected = new StringBuilder();
            expected.Append(
                @"[When(@""I can parse integers such as """"(\d+)"""", """"(\d+)"""" and """"(\d+)"""""")]
public void WhenICanParseIntegersSuchAsAnd(int int1, int int2, int int3)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                "I can parse integers such as \"20\", \"2147483647\" and \"-9\"", null, null, null);

            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Parsed_Booleans()
        {
            var expected = new StringBuilder();
            expected.Append(
                @"[Given(@""I can parse booleans such as """"(True|False)"""" and """"(True|False)"""""")]
public void GivenICanParseBooleansSuchAsAnd(bool bool1, bool bool2)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                "I can parse booleans such as \"True\" and \"False\"", null, null, null);
            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Unrecognised_Parameters_Parsed_As_Strings()
        {
            var expected = new StringBuilder();
            expected.Append(
                @"[When(@""I can parse strings such as """"(.*)"""", """"(.*)"""" and """"(.*)"""""")]
public void WhenICanParseStringsSuchAsAnd(string string1, string string2, string string3)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                "I can parse strings such as \"20b\", \"cactus\" and \"*$&£(^*(Q\"", null, null, null);

            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Method_Skeleton_With_Parsed_Parameters()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Given(@""String """"(.*)"""" int """"(\d+)"""" dateTime """"(.*)"""" bool """"(True|False)"""" double """"(.*)"""""")]
public void GivenStringIntDateTimeBoolDouble(string multilineText, Table table, string string1, int int2, DateTime dateTime3, bool bool4, double double5)
{
    ScenarioContext.Current.Pending();
}
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                        "String \"cactus\" int \"32\" dateTime \"13:09\" bool \"True\" double \"3.1415\"",
                                        "Cat\r\ndog", new Table("Test", "Table"), null);
            string result = skeletonProviderCS.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_CSharp_Class_Skeleton()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Binding]
public class StepDefinitions
{
    
    #region Given
    
    [Given(@""I have a new step"")]
    public void GivenIHaveANewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
}
");
            var steps = new List<StepArgs>
               {
                   new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                       "I have a new step", null, null, null),
               };

            string output = skeletonProviderCS.GetBindingClassSkeleton(steps);

            Assert.AreEqual(expected.ToString(), output);
        }

        [Test]
        public void Produces_CSharp_Class_Skeleton_With_All_Step_Types()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Binding]
public class StepDefinitions
{
    
    #region Given
    
    [Given(@""I have a new step"")]
    public void GivenIHaveANewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
    #region When
    
    [When(@""I do not find a step binding"")]
    public void WhenIDoNotFindAStepBinding()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
    #region Then
    
    [Then(@""I get a suggestion"")]
    public void ThenIGetASuggestion()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
}
");
            var steps = new List<StepArgs>
                {
                    new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, "I have a new step", null, null, null),
                    new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, "I get a suggestion", null, null, null),
                    new StepArgs(BindingType.When, StepDefinitionKeyword.When, "I do not find a step binding", null, null, null)
                };

            string output = skeletonProviderCS.GetBindingClassSkeleton(steps);

            Assert.AreEqual(expected.ToString(), output);
        }

        [Test]
        public void Produces_CSharp_Class_Skeleton_With_Multiple_Steps_Of_The_Same_Type()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Binding]
public class StepDefinitions
{
    
    #region Then
    
    [Then(@""I have a new step"")]
    public void ThenIHaveANewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    [Then(@""I have a second new step"")]
    public void ThenIHaveASecondNewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
}
");
            var steps = new List<StepArgs>
            {
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, "I have a new step",
                    null, null, null),
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, "I have a second new step",
                    null, null, null),
            };
            string output = skeletonProviderCS.GetBindingClassSkeleton(steps);
            Assert.AreEqual(expected.ToString(), output);
        }

        [Test]
        public void Produces_CSharp_Class_Skeleton_With_Multiple_Steps_Of_Multiple_Types()
        {
            var expected = new StringBuilder();
            expected.Append(
@"[Binding]
public class StepDefinitions
{
    
    #region When
    
    [When(@""one"")]
    public void WhenOne()
    {
        ScenarioContext.Current.Pending();
    }
    
    [When(@""two"")]
    public void WhenTwo()
    {
        ScenarioContext.Current.Pending();
    }
    
    [When(@""three"")]
    public void WhenThree()
    {
        ScenarioContext.Current.Pending();
    }
    
    [When(@""four"")]
    public void WhenFour()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
    #region Then
    
    [Then(@""I have a new step"")]
    public void ThenIHaveANewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    [Then(@""I have a second new step"")]
    public void ThenIHaveASecondNewStep()
    {
        ScenarioContext.Current.Pending();
    }
    
    #endregion
    
}
");
            var steps = new List<StepArgs>
            {
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                    "I have a new step", null, null, null),
                new StepArgs(BindingType.Then, StepDefinitionKeyword.And,
                    "I have a second new step", null, null, null),
                new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                    "one", null, null, null),
                new StepArgs(BindingType.When, StepDefinitionKeyword.And,
                    "two", null, null, null),
                new StepArgs(BindingType.When, StepDefinitionKeyword.And,
                    "three", null, null, null),
                new StepArgs(BindingType.When, StepDefinitionKeyword.And,
                    "four", null, null, null),
            };
            string output = skeletonProviderCS.GetBindingClassSkeleton(steps);
            Assert.AreEqual(expected.ToString(), output);
        }

        [Test]
        public void Produces_CSharp_Class_File_Skeleton()
        {
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have a new step"")]
        public void GivenIHaveANewStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I get a suggestion"")]
        public void ThenIGetASuggestion()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}
");
            string expectedString = expected.ToString();
            var steps = new List<StepArgs>
                {
                    new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                     "I have a new step", null, null, null),
                    new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                     "I get a suggestion", null, null, null),
                };
            var skelInfo = new StepDefSkeletonInfo("StepsForTest", "SpecFlow.Testing");
            string output = skeletonProviderCS.GetFileSkeleton(steps, skelInfo);

            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Add_Step_To_Existing_Empty_CSharp_Class_File()
        {
            var file = new StringBuilder();
            file.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {

    }
}");
            string fileText = file.ToString();
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have a new step"")]
        public void GivenIHaveANewStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion

    }
}");
            string expectedOutput = expected.ToString();

            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And, 
                                    "I have a new step", null, null, null)
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Steps_Of_Multiple_Types_To_Existing_Empty_CSharp_Class_File()
        {
            var file = new StringBuilder();
            file.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {

    }
}");
            string fileText = file.ToString();
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have a new step"")]
        public void GivenIHaveANewStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region When
        
        [When(@""I have a when step"")]
        public void WhenIHaveAWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I have a then step"")]
        public void ThenIHaveAThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion

    }
}");
            string expectedOutput = expected.ToString();

            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                                 "I have a new step", null, null, null),
                                new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                                                 "I have a when step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                                 "I have a then step", null, null, null),
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Multiple_Steps_Of_Muliple_Types_To_Existing_Empty_CSharp_Class_File()
        {
            var file = new StringBuilder();
            file.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {

    }
}");
            string fileText = file.ToString();
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region When
        
        [When(@""I have a when step"")]
        public void WhenIHaveAWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""I have another when step"")]
        public void WhenIHaveAnotherWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I have a then step"")]
        public void ThenIHaveAThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""I have another then step"")]
        public void ThenIHaveAnotherThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion

    }
}");
            string expectedOutput = expected.ToString();

            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                                                 "I have a when step",  null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                                 "I have a then step",  null, null, null),
                                new StepArgs(BindingType.When, StepDefinitionKeyword.And,
                                                 "I have another when step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.And,
                                                 "I have another then step",  null, null, null),
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Step_To_Existing_CSharp_Class_File_Inside_Region()
        {
            var original = new StringBuilder();
            original.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have a new step"")]
        public void GivenIHaveANewStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I get a suggestion"")]
        public void ThenIGetASuggestion()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And,
                                    "I have multiple steps", null, null, null)
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have multiple steps"")]
        public void GivenIHaveMultipleSteps()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@""I have a new step"")]
        public void GivenIHaveANewStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I get a suggestion"")]
        public void ThenIGetASuggestion()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Steps_Of_Muliple_Types_To_Existing_CSharp_Class_File_With_Multiple_Steps()
        {
            var file = new StringBuilder();
            file.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have a given step"")]
        public void GivenIHaveAGivenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region When
        
        [When(@""I have a when step"")]
        public void WhenIHaveAWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I have a then step"")]
        public void ThenIHaveAThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}");
            string fileText = file.ToString();
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""I have another given step"")]
        public void GivenIHaveAnotherGivenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@""I have a given step"")]
        public void GivenIHaveAGivenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region When
        
        [When(@""I have another when step"")]
        public void WhenIHaveAnotherWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""I have a when step"")]
        public void WhenIHaveAWhenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""I have another then step"")]
        public void ThenIHaveAnotherThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""I have a then step"")]
        public void ThenIHaveAThenStep()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}");
            string expectedOutput = expected.ToString();

            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                    "I have another given step", null, null, null),
                                new StepArgs(BindingType.When, StepDefinitionKeyword.And,
                                    "I have another when step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.And,
                                    "I have another then step", null, null, null),
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Multiple_Steps_Of_Muliple_Types_To_Existing_CSharp_Class_File_With_Multiple_Steps()
        {
            var file = new StringBuilder();
            file.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region When
        
        [When(@""one"")]
        public void WhenOne()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""two"")]
        public void WhenTwo()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""three"")]
        public void WhenThree()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""one"")]
        public void ThenOne()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""two"")]
        public void ThenTwo()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}");
            string fileText = file.ToString();
            var expected = new StringBuilder();
            expected.Append(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace SpecFlow.Testing
{
    [Binding]
    public class StepsForTest
    {
        
        #region Given
        
        [Given(@""one"")]
        public void GivenOne()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@""two"")]
        public void GivenTwo()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region When
        
        [When(@""four"")]
        public void WhenFour()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""one"")]
        public void WhenOne()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""two"")]
        public void WhenTwo()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@""three"")]
        public void WhenThree()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
        #region Then
        
        [Then(@""three"")]
        public void ThenThree()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""four"")]
        public void ThenFour()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""one"")]
        public void ThenOne()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@""two"")]
        public void ThenTwo()
        {
            ScenarioContext.Current.Pending();
        }
        
        #endregion
        
    }
}");
            string expectedOutput = expected.ToString();

            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                    "one", null, null, null),
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And, 
                                    "two", null, null, null),
                                new StepArgs(BindingType.When, StepDefinitionKeyword.And, 
                                    "four", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.And, 
                                    "three", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.And, 
                                    "four", null, null, null)
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Steps_To_Invalid_CSharp_Class_File_Returns_Null()
        {
            string fileText = "Not a class";
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And,
                                                 "I have multiple steps", null, null, null),
                            };
            var actualOutput = skeletonProviderCS.AddStepsToExistingFile(fileText, steps);
            Assert.IsNull(actualOutput);
        }

        #endregion

        #region VBasic
        [Test]
        public void Produces_VBasic_Method_Skeleton()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Given(""I have a new step"")> _
Public Sub GivenIHaveANewStep()
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                                         "I have a new step", null, null, null);

            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Parsed_Doubles()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Given(""I can parse a double """"(.*)"""" and another double """"(.*)"""""")> _
Public Sub GivenICanParseADoubleAndAnotherDouble(ByVal double1 As Double, ByVal double2 As Double)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                "I can parse a double \"3.4\" and another double \"902.302\"", null, null, null);

            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Parsed_Dates()
        {
            var expected = new StringBuilder();
            expected.Append(
                @"<Given(""I can parse a date """"(.*)"""" and another date """"(.*)"""""")> _
Public Sub GivenICanParseADateAndAnotherDate(ByVal date1 As Date, ByVal date2 As Date)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                "I can parse a date \"23/01/96\" and another date \"13:00 GMT 20/02/01\"", null, null, null);

            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Parsed_Ints()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<TechTalk.SpecFlow.When(""I can parse integers such as """"(\d+)"""", """"(\d+)"""" and """"(\d+)"""""")> _
Public Sub WhenICanParseIntegersSuchAsAnd(ByVal integer1 As Integer, ByVal integer2 As Integer, ByVal integer3 As Integer)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                "I can parse integers such as \"20\", \"2147483647\" and \"-9\"", null, null, null);

            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Parsed_Booleans()
        {
            var expected = new StringBuilder();
            expected.Append(
                @"<Given(""I can parse booleans such as """"(True|False)"""" and """"(True|False)"""""")> _
Public Sub GivenICanParseBooleansSuchAsAnd(ByVal boolean1 As Boolean, ByVal boolean2 As Boolean)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                "I can parse booleans such as \"True\" and \"False\"", null, null, null);
            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Unrecognised_Parameters_Parsed_As_Strings()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Given(""I can parse strings such as """"(.*)"""", """"(.*)"""" and """"(.*)"""""")> _
Public Sub GivenICanParseStringsSuchAsAnd(ByVal string1 As String, ByVal string2 As String, ByVal string3 As String)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                "I can parse strings such as \"20b\", \"cactus\" and \"*$&£(^*(Q\"", null, null, null);

            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Method_Skeleton_With_Parsed_Parameters()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Given(""String """"(.*)"""" int """"(\d+)"""" dateTime """"(.*)"""" bool """"(True|False)"""" double """"(.*)"""""")> _
Public Sub GivenStringIntDateTimeBoolDouble(ByVal multilineText As String, ByVal table As Table, ByVal string1")
                .Append
(@" As String, ByVal integer2 As Integer, ByVal date3 As Date, ByVal boolean4 As Boolean, ByVal double5 As Double)
    ScenarioContext.Current.Pending()
End Sub
");
            var stepArgs = new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                        "String \"cactus\" int \"32\" dateTime \"13:09\" bool \"True\" double \"3.1415\"",
                                        "Cat\r\ndog", new Table("Test", "Table"), null);
            string result = skeletonProviderVB.GetStepDefinitionSkeleton(stepArgs);
            Assert.AreEqual(expected.ToString(), result);
        }

        [Test]
        public void Produces_VBasic_Class_Skeleton()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Binding> _
Public Class StepDefinitions
    
    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedString = expected.ToString();

            var steps = new List<StepArgs>
            {
                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                    "I have a new step", null ,null, null),
            };

            string output = skeletonProviderVB.GetBindingClassSkeleton(steps);

            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Produces_VBasic_Class_Skeleton_With_Multiple_Steps()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Binding> _
Public Class StepDefinitions
    
    <TechTalk.SpecFlow.Then(""I have a new step"")> _
    Public Sub ThenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have another new step"")> _
    Public Sub ThenIHaveAnotherNewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedString = expected.ToString();
            var steps = new List<StepArgs>
            {
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                    "I have a new step", null, null, null),
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                    "I have another new step", null, null, null),
            };
            string output = skeletonProviderVB.GetBindingClassSkeleton(steps);
            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Produces_VBasic_Class_Skeleton_With_All_Step_Types()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Binding> _
Public Class StepDefinitions
    
    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedString = expected.ToString();

            var steps = new List<StepArgs>
                 {
                     new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, "I have a new step",
                         null, null, null),
                     new StepArgs(BindingType.When, StepDefinitionKeyword.When, "I do not find a step binding",
                         null, null, null),
                     new StepArgs(BindingType.Then, StepDefinitionKeyword.Then, "I get a suggestion",
                         null, null, null)
                 };

            string output = skeletonProviderVB.GetBindingClassSkeleton(steps);
            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Produces_VBasic_Class_Skeleton_With_Multiple_Steps_Of_Multiple_Types()
        {
            var expected = new StringBuilder();
            expected.Append(
@"<Binding> _
Public Class StepDefinitions
    
    <Given(""one"")> _
    Public Sub GivenOne()
        ScenarioContext.Current.Pending()
    End Sub
    
    <Given(""two"")> _
    Public Sub GivenTwo()
        ScenarioContext.Current.Pending()
    End Sub
    
    <Given(""three"")> _
    Public Sub GivenThree()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have a new step"")> _
    Public Sub ThenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have another new step"")> _
    Public Sub ThenIHaveAnotherNewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedString = expected.ToString();
            var steps = new List<StepArgs>
            {
                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                    "one", null, null, null),
                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                    "two", null, null, null),
                new StepArgs(BindingType.Given, StepDefinitionKeyword.And,
                    "three", null, null, null),
                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                    "I have a new step", null, null, null),
                new StepArgs(BindingType.Then, StepDefinitionKeyword.And,
                    "I have another new step", null, null, null),
            };
            string output = skeletonProviderVB.GetBindingClassSkeleton(steps);
            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Produces_VBasic_Class_File_Skeleton()
        {
            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest
    
    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedString = expected.ToString();

            var steps = new List<StepArgs>
                {
                    new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                        "I have a new step", null, null, null),
                    new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                        "I do not find a step binding", null, null, null),
                    new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                        "I get a suggestion", null, null, null)
                };
            var skelInfo = new StepDefSkeletonInfo("StepsForTest", "namespace");
            string output = skeletonProviderVB.GetFileSkeleton(steps, skelInfo);

            Assert.AreEqual(expectedString, output);
        }

        [Test]
        public void Add_Step_To_Existing_Empty_VBasic_Class_File()
        {
            var original = new StringBuilder();
            original.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

End Class
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And, 
                                    "I have a new step", null, null, null)
                            };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Multiple_Steps_To_Existing_Empty_VBasic_Class_File()
        {
            var original = new StringBuilder();
            original.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

End Class
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                {
                    new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                     "I have a new step", null, null, null),
                    new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                                     "I have a new step", null, null, null),
                    new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                     "I have a new step", null, null, null)
                };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.When(""I have a new step"")> _
    Public Sub WhenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have a new step"")> _
    Public Sub ThenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Multiple_Steps_Of_Multiple_Types_To_Existing_Empty_VBasic_Class_File()
        {
            var original = new StringBuilder();
            original.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

End Class
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                                 "I have a new step", null, null, null),
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given,
                                                 "I have another new step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                                 "I have a new step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                                 "I have another new step", null, null, null)
                            };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest

    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <Given(""I have another new step"")> _
    Public Sub GivenIHaveAnotherNewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have a new step"")> _
    Public Sub ThenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I have another new step"")> _
    Public Sub ThenIHaveAnotherNewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Step_To_Existing_VBasic_Class_File()
        {
            var original = new StringBuilder();
            original.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And,
                                                 "I have a new step", null, null, null)
                            };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
    <Given(""I have a new step"")> _
    Public Sub GivenIHaveANewStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Multiple_Steps_To_Existing_VBasic_Class_File()
        {
            var original = new StringBuilder();
            original.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string fileText = original.ToString();
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.Given, 
                                    "I add a given step", null, null, null),
                                new StepArgs(BindingType.When, StepDefinitionKeyword.When,
                                    "I add a when step", null, null, null),
                                new StepArgs(BindingType.Then, StepDefinitionKeyword.Then,
                                    "I add a then step", null, null, null)
                            };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);

            var expected = new StringBuilder();
            expected.Append(
@"Imports TechTalk.SpecFlow

<Binding> _
Public Class StepsForTest
    
    <TechTalk.SpecFlow.When(""I do not find a step binding"")> _
    Public Sub WhenIDoNotFindAStepBinding()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I get a suggestion"")> _
    Public Sub ThenIGetASuggestion()
        ScenarioContext.Current.Pending()
    End Sub
    
    <Given(""I add a given step"")> _
    Public Sub GivenIAddAGivenStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.When(""I add a when step"")> _
    Public Sub WhenIAddAWhenStep()
        ScenarioContext.Current.Pending()
    End Sub
    
    <TechTalk.SpecFlow.Then(""I add a then step"")> _
    Public Sub ThenIAddAThenStep()
        ScenarioContext.Current.Pending()
    End Sub
    
End Class
");
            string expectedOutput = expected.ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Add_Steps_To_Invalid_VBasic_Class_File_Returns_Null()
        {
            string fileText = "Not a class";
            var steps = new List<StepArgs>
                            {
                                new StepArgs(BindingType.Given, StepDefinitionKeyword.And,
                                                 "I have multiple steps", null, null, null)
                            };
            var actualOutput = skeletonProviderVB.AddStepsToExistingFile(fileText, steps);
            Assert.IsNull(actualOutput);
        }

        #endregion

    }
}

