using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static bool operator ==(Employee left, Employee right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Employee left, Employee right)
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
            return Equals((Employee)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FirstName != null ? FirstName.GetHashCode() : 0) * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
            }
        }

        protected bool Equals(Employee other)
        {
            return string.Equals(FirstName, other.FirstName) && string.Equals(LastName, other.LastName);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public BankAccount BankAccount { get; set; }
    }

    public class BankAccount
    {
        public int AccountNumber { get; set; }
    }

    [Binding]
    public class UserConverter
    {
        [StepArgumentTransformation(@"user (\w+)")]
        public User CreateUser(string name)
        {
            return new User { Name = name };
        }

        [StepArgumentTransformation]
        public IEnumerable<User> CreateUsers(Table table)
        {
            return table.Rows.Select(tableRow =>
                new User { Name = tableRow["Name"] });
        }

        [StepArgumentTransformation(@"user (\w+) with account (\w+)")]
        public User CreateUserWithBankAccount(string name, BankAccount bankAccount)
        {
            return new User { Name = name, BankAccount = bankAccount };
        }

        [StepArgumentTransformation]
        public BankAccount CreateBankAccount(int accountNumber)
        {
            return new BankAccount { AccountNumber = accountNumber };
        }
    }

    [Binding]
    public class EmployeeCreator
    {
        [StepArgumentTransformation]
        public Employee CreateEmployee(string firstName, string lastName)
        {
            return new Employee { FirstName = firstName, LastName = lastName };
        }
    }

    [TestFixture]
    public class StepTransformationTests
    {
        private readonly Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
        private readonly Mock<IContextManager> contextManagerStub = new Mock<IContextManager>();
        private readonly Mock<IBindingInvoker> methodBindingInvokerStub = new Mock<IBindingInvoker>();
        private readonly List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();

        [SetUp]
        public void SetUp()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            var scenarioContext = new ScenarioContext(null, null, null);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);
            stepTransformations.Clear();
            stepTransformations.AddRange(typeof(UserConverter).GetMethods().Where(x => x.GetCustomAttributes(typeof(StepArgumentTransformationAttribute), false).Length>0).Select(CreateStepTransformationBinding));
            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(MethodInfo transformMethod)
        {
            var regexString = transformMethod.GetCustomAttributes(typeof(StepArgumentTransformationAttribute), false)
                .OfType<StepArgumentTransformationAttribute>().Select(x => x.Regex).FirstOrDefault();
            return new StepArgumentTransformationBinding(regexString, new RuntimeBindingMethod(transformMethod));
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(string regexString, IBindingMethod transformMethod)
        {
            return new StepArgumentTransformationBinding(regexString, transformMethod);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(string regexString, MethodInfo transformMethod)
        {
            return new StepArgumentTransformationBinding(regexString, new RuntimeBindingMethod(transformMethod));
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationForStringConversionIfRegexDoesNotMatch()
        {            
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();

            Queue<object> values = new Queue<object>();
            values.Enqueue("xyz");
            var result = stepArgumentTypeConverter.CanConvert(values, new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationForStringConversionIfRegexMatchesButArgumentsCannotBeConvertedToParameterTypes()
        {
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();
            Queue<object> values = new Queue<object>();
            values.Enqueue("user xyz with account abcd");
            var result = stepArgumentTypeConverter.CanConvert(values, new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseStepArgumentTransformationForStringConversionIfRegexMatchesAndArgumentsCanBeConvertedToParameterTypes()
        {
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();
            Queue<object> values = new Queue<object>();
            values.Enqueue("user xyz with account 1234");
            var result = stepArgumentTypeConverter.CanConvert(values, new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.True);
        }

        [Test]
        public void UserConverterShouldConvertStringToUser()
        {
            UserConverter stepTransformationInstance = new UserConverter();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("CreateUser");
            var stepTransformationBinding = CreateStepTransformationBinding(@"user (\w+)", transformMethod);

            Assert.True(stepTransformationBinding.Regex.IsMatch("user xyz"));

            var invoker = new BindingInvoker(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object);
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "xyz" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(User)));
            Assert.That(((User)result).Name, Is.EqualTo("xyz"));
        }

        [Test]
        public void StepArgumentTransformationShouldConvertNumberToBankAccount()
        {
            UserConverter stepTransformationInstance = new UserConverter();
            var transformMethod = stepTransformationInstance.GetType().GetMethod("CreateBankAccount");
            var stepTransformationBinding = CreateStepTransformationBinding(@"", transformMethod);

            var invoker = new BindingInvoker(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object);
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { 1234 }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(BankAccount)));
            Assert.That(((BankAccount)result).AccountNumber, Is.EqualTo(1234));
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationToConvertArgumentsOfOtherStepArgumentTransformation()
        {
            var bankAccountStepTransformationBinding = stepTransformations.First(x => x.Method.Name == "CreateBankAccount");
            var userStepTransformationBinding = stepTransformations.First(x => x.Method.Name == "CreateUserWithBankAccount");

            var resultBankAccount = new BankAccount { AccountNumber = 1234 };
            var resultUser = new User { Name = "xyz", BankAccount = resultBankAccount };

            TimeSpan duration;
            
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(bankAccountStepTransformationBinding, It.IsAny<IContextManager>(), new object[] { 1234 }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultBankAccount);
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(userStepTransformationBinding, It.IsAny<IContextManager>(), new object[] { "xyz", resultBankAccount }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();
            var result = stepArgumentTypeConverter.Convert("user xyz with account 1234", typeof(User), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultUser));
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseUserConverterForConversion()
        {
            var stepTransformationBinding = stepTransformations.First(x => x.Method.Name == "CreateUser");
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUser = new User();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result, Is.EqualTo(resultUser));
        }

        private StepArgumentTypeConverter CreateStepArgumentTypeConverter()
        {
            return new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, contextManagerStub.Object, methodBindingInvokerStub.Object);
        }

        [Test]
        public void ShouldUseStepArgumentTransformationToConvertTable()
        {
            var table = new Table("Name");

            var stepTransformationBinding = stepTransformations.First(x => x.Method.Name == "CreateUsers");
            stepTransformations.Add(stepTransformationBinding);
            TimeSpan duration;
            var resultUsers = new User[3];
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), new object[] { table }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUsers);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter();


            var result = stepArgumentTypeConverter.Convert(table, typeof(IEnumerable<User>), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultUsers));
        }

        [Test]
        public void EmployeeConverterShouldConvertMultipleStringsToEmployee()
        {
            var stepTransformationInstance = new EmployeeCreator();
            MethodInfo transformMethod = stepTransformationInstance.GetType().GetMethod("CreateEmployee");
            IStepArgumentTransformationBinding stepTransformationBinding = CreateStepTransformationBinding(@"", transformMethod);

            var invoker = new BindingInvoker(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object);
            TimeSpan duration;
            object result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { "John", "Smith" }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(Employee)));
            Assert.That(((Employee)result).FirstName, Is.EqualTo("John"));
            Assert.That(((Employee)result).LastName, Is.EqualTo("Smith"));
        }
    }

}