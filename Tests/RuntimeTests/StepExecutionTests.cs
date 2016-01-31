using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using TechTalk.SpecFlow.Infrastructure;
using TestStatus = TechTalk.SpecFlow.Infrastructure.TestStatus;

namespace TechTalk.SpecFlow.RuntimeTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [Binding]
    public class StepExecutionTestsBindings
    {
        [Given("sample step without param")]
        public virtual void BindingWithoutParam()
        {

        }

        [Given("sample step with (single) param")]
        public virtual void BindingWithSingleParam(string param)
        {

        }

        [Given("sample step with (multi)(ple) param")]
        public virtual void BindingWithMultipleParam(string param1, string param2)
        {

        }

        [Given("sample step with table param")]
        public virtual void BindingWithTableParam(Table table)
        {

        }

        [Given("sample step with multi-line string param")]
        public virtual void BindingWithMlStringParam(string multiLineString)
        {

        }

        [Given("sample step with table and multi-line string param")]
        public virtual void BindingWithTableAndMlStringParam(string multiLineString, Table table)
        {

        }

        [Given("sample step with (mixed) params")]
        public virtual void BindingWithMixedParams(string param1, string multiLineString, Table table)
        {

        }

        [Given("sample step with simple convert param: (.*)")]
        public virtual void BindingWithSimpleConvertParam(double param)
        {

        }

        [Given("sample step with wrong param number")]
        public virtual void BindingWithWrongParamNumber(double param)
        {

        }

        [Given("Distinguish by table param")]
        public virtual void DistinguishByTableParam1()
        {

        }

        [Given("Distinguish by table param")]
        public virtual void DistinguishByTableParam2(Table table)
        {

        }
    }

    [Binding]
    public class StepExecutionTestsStepArgumentTransformation
    {
        [Given("sample step with user param firstname '(.*)'")]
        public virtual void BindingWithoutParam1(User user)
        {
        }

        [StepArgumentTransformation]
        public User TransformToEmployee(string firstName)
        {
            return new User { Name = firstName };
        }
    }

    [Binding]
    public class StepExecutionTestsAmbiguousBindings
    {
        [Given("sample step without param")]
        public virtual void BindingWithoutParam1()
        {

        }

        [Given("sample step without param")]
        public virtual void BindingWithoutParam2()
        {

        }
    }

    [Binding]
    public class StepExecutionTestsWithRegexBindings
    {
        [StepArgumentTransformation(@"in (\d+) days")]
        public DateTime ConvertInDays(int days)
        {
            return DateTime.Today.AddDays(days);
        }
        [Given(@"I have an appointment (.*)")]
        public virtual void GivenIHaveAnAppointmentAt(DateTime time)
        { }

    }

    [Binding]
    public class StepExecutionTestsStepArgumentTransformationWithMultipleParameters
    {
        [Given("sample step with user param firstname '(.*)' lastname '(.*)'")]
        public virtual void BindingWithMultipleStringParamStepTransformation(Employee employee)
        {
        }

        [Given("sample step with user param firstint (.*) lastint (.*)")]
        public virtual void BindingWithMultipleIntParamStepTransformation(IntThing employee)
        {
        }

        [Given("sample step with user param firstdouble (.*) lastdouble (.*)")]
        public virtual void BindingWithMultipleDoubleParamStepTransformation(DoubleThing doubleThing)
        {
        }

        [Given("sample step with table thing aDouble (.*) anInt (.*) a string '(.*)' then a table")]
        public virtual void BindingWithMultipleParamIncludingTableStepTransformation(TableThing tableThing)
        {
        }

        [Given("sample step with string '(.*)' param firstdouble (.*) lastdouble (.*) then firstname '(.*)' lastname '(.*)' then a int (.*) follwed by param firstint (.*) lastint (.*) and finally a string '(.*)'")]
        public virtual void BindingWithManyMultipleParamStepTransformation(String name, DoubleThing doubleThing, Employee employee, int justAnInt, IntThing intThing, string anotherString)
        {
        }

        [StepArgumentTransformation]
        public Employee TransformToEmployee(string firstName, string lastName)
        {
            return new Employee { FirstName = firstName, LastName = lastName };
        }

        [StepArgumentTransformation]
        public IntThing TransformToIntThing(int firstInt, int lastInt)
        {
            return new IntThing { FirstInt = firstInt, LastInt = lastInt };
        }

        [StepArgumentTransformation]
        public DoubleThing TransformToDoubleThing(double firstDouble, double lastDouble)
        {
            return new DoubleThing { FirstDouble = firstDouble, LastDouble = lastDouble };
        }

        [StepArgumentTransformation]
        public TableThing TransformToDoubleThing(double aDouble, int anInt, string aString, Table aTable)
        {
            return new TableThing { MyDouble = aDouble, MyInt = anInt, MyString = aString, TableHeader = aTable.Header };
        }
    }

    public class TableThing
    {
        public double MyDouble { get; set; }
        public int MyInt { get; set; }
        public string MyString { get; set; }
        public ICollection<string> TableHeader { get; set; }

        protected bool Equals(TableThing other)
        {
            return MyDouble.Equals(other.MyDouble) && MyInt == other.MyInt && string.Equals(MyString, other.MyString) && CollectionsAreEqual(other);
        }

        private bool CollectionsAreEqual(TableThing other)
        {
            if (TableHeader.Count != other.TableHeader.Count)
            {
                return false;
            }
            for (int i = 0; i < TableHeader.Count; i++)
            {
                if (TableHeader.ElementAt(i) != other.TableHeader.ElementAt(i))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((TableThing)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MyDouble.GetHashCode();
                hashCode = (hashCode * 397) ^ MyInt;
                hashCode = (hashCode * 397) ^ (MyString != null ? MyString.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TableHeader != null ? TableHeader.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class DoubleThing
    {
        public double FirstDouble { get; set; }
        public double LastDouble { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DoubleThing)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirstDouble.GetHashCode() * 397) ^ LastDouble.GetHashCode();
            }
        }

        protected bool Equals(DoubleThing other)
        {
            return FirstDouble.Equals(other.FirstDouble) && LastDouble.Equals(other.LastDouble);
        }
    }

    public class IntThing
    {
        public int FirstInt { get; set; }
        public int LastInt { get; set; }

        public static bool operator ==(IntThing left, IntThing right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntThing left, IntThing right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((IntThing)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FirstInt * 397) ^ LastInt;
            }
        }

        protected bool Equals(IntThing other)
        {
            return FirstInt == other.FirstInt && LastInt == other.LastInt;
        }
    }

    [TestFixture]
    public class StepExecutionTests : StepExecutionTestsBase
    {
        [Test]
        public void SholdCallBindingWithoutParameter()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithoutParam());

            MockRepository.ReplayAll();

            testRunner.Given("sample step without param");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingSingleParameter()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithSingleParam("single"));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with single param");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingMultipleParameter()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithMultipleParam("multi", "ple"));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with multiple param");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingWithTableParameter()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            bindingInstance.Expect(b => b.BindingWithTableParam(table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with table param", null, table);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingWithMlStringParam()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            const string mlString = "ml-string";
            bindingInstance.Expect(b => b.BindingWithMlStringParam(mlString));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with multi-line string param", mlString, null);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingWithTableAndMlStringParam()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            const string mlString = "ml-string";
            bindingInstance.Expect(b => b.BindingWithTableAndMlStringParam(mlString, table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with table and multi-line string param", mlString, table);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdCallBindingWithMixedParams()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            const string mlString = "ml-string";
            bindingInstance.Expect(b => b.BindingWithMixedParams("mixed", mlString, table));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with mixed params", mlString, table);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdRaiseAmbiguousIfMultipleMatch()
        {
            StepExecutionTestsAmbiguousBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            MockRepository.ReplayAll();

            testRunner.Given("sample step without param");

            Assert.AreEqual(TestStatus.BindingError, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdDistinguishByTableParam_CallWithoutTable()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.DistinguishByTableParam1());

            MockRepository.ReplayAll();

            testRunner.Given("Distinguish by table param");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdDistinguishByTableParam_CallWithTable()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            Table table = new Table("h1");
            bindingInstance.Expect(b => b.DistinguishByTableParam2(table));

            MockRepository.ReplayAll();

            testRunner.Given("Distinguish by table param", null, table);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void SholdRaiseBindingErrorIfWrongParamNumber()
        {
            StepExecutionTestsBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            MockRepository.ReplayAll();

            testRunner.Given("sample step with wrong param number");

            Assert.AreEqual(TestStatus.BindingError, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformation bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithoutParam1(Arg<User>.Is.NotNull));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with user param firstname 'John'");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepWithMultipleDoubleParamsIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformationWithMultipleParameters bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithMultipleDoubleParamStepTransformation(Arg<DoubleThing>.Is.Equal(new DoubleThing { FirstDouble = 10.1, LastDouble = 15.8 })));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with user param firstdouble 10.1 lastdouble 15.8");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepWithMultipleIntParamsIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformationWithMultipleParameters bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithMultipleIntParamStepTransformation(Arg<IntThing>.Is.Equal(new IntThing { FirstInt = 10, LastInt = 15 })));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with user param firstint 10 lastint 15");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepWithMultipleParamsIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformationWithMultipleParameters bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithMultipleStringParamStepTransformation(Arg<Employee>.Is.Equal(new Employee { FirstName = "John", LastName = "Smith" })));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with user param firstname 'John' lastname 'Smith'");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepWithMultipleParamsIncludingTableIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformationWithMultipleParameters bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithMultipleParamIncludingTableStepTransformation(Arg<TableThing>.Is.Equal(new TableThing { MyDouble = 11.0, MyInt = 12, MyString = "Bob", TableHeader = new Collection<string>() { "Table", "Header" } })));

            MockRepository.ReplayAll();

            Table table = new Table("Table", "Header");
            testRunner.Given("sample step with table thing aDouble 11.0 anInt 12 a string 'Bob' then a table", null, table);

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWhenStepWithMultipleParamsOfDifferntTypesIsTransformed()
        {
            StepExecutionTestsStepArgumentTransformationWithMultipleParameters bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.BindingWithManyMultipleParamStepTransformation(Arg<String>.Is.Equal("justAString"),
                    Arg<DoubleThing>.Is.Equal(new DoubleThing() { FirstDouble = 11.6, LastDouble = 10.5 }),
                    Arg<Employee>.Is.Equal(new Employee { FirstName = "John", LastName = "Smith" }),
                    Arg<int>.Is.Equal(87),
                    Arg<IntThing>.Is.Equal(new IntThing() { FirstInt = 65, LastInt = 66 }),
                    Arg<string>.Is.Equal("finalString")
                    ));

            MockRepository.ReplayAll();

            testRunner.Given("sample step with string 'justAString' param firstdouble 11.6 lastdouble 10.5 then firstname 'John' lastname 'Smith' then a int 87 follwed by param firstint 65 lastint 66 and finally a string 'finalString'");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }

        [Test]
        public void ShouldCallBindingWitRegexTransformation()
        {
            StepExecutionTestsWithRegexBindings bindingInstance;
            TestRunner testRunner = GetTestRunnerFor(out bindingInstance);

            bindingInstance.Expect(b => b.GivenIHaveAnAppointmentAt(DateTime.Today.AddDays(2)));

            MockRepository.ReplayAll();

            testRunner.Given("I have an appointment in 2 days");

            Assert.AreEqual(TestStatus.OK, GetLastTestStatus());
            MockRepository.VerifyAll();
        }
    }
}
