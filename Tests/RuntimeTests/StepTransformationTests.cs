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
            return new User {Name = name};
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

    [TestFixture]
    public class StepTransformationTests
    {
        private readonly Mock<IBindingRegistry> bindingRegistryStub = new Mock<IBindingRegistry>();
        private readonly Mock<IContextManager> contextManagerStub = new Mock<IContextManager>();
        private readonly List<IStepArgumentTransformationBinding> stepTransformations = new List<IStepArgumentTransformationBinding>();

        [SetUp]
        public void SetUp()
        {
            // ScenarioContext is needed, because the [Binding]-instances live there
            var scenarioContext = new ScenarioContext(null, null, null);
            contextManagerStub.Setup(cm => cm.ScenarioContext).Returns(scenarioContext);

            stepTransformations.Clear();
            stepTransformations.AddRange(typeof(UserConverter).GetMethods().Select(CreateStepTransformationBinding));

            bindingRegistryStub.Setup(br => br.GetStepTransformations()).Returns(stepTransformations);
        }

        private IStepArgumentTransformationBinding CreateStepTransformationBinding(MethodInfo transformMethod)
        {
            var regexString = transformMethod.GetCustomAttributes(typeof(StepArgumentTransformationAttribute), false)
                .OfType<StepArgumentTransformationAttribute>().Select(x => x.Regex).FirstOrDefault();
            return new StepArgumentTransformationBinding(regexString, new RuntimeBindingMethod(transformMethod));
        }

        private StepArgumentTypeConverter CreateStepArgumentTypeConverter(IBindingInvoker bindingInvoker)
        {
            return new StepArgumentTypeConverter(new Mock<ITestTracer>().Object, bindingRegistryStub.Object, contextManagerStub.Object, bindingInvoker);
        }

        [Test]
        public void StepArgumentTransformationShouldConvertStringToUser()
        {
            var stepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateUser");

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
            var stepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateBankAccount");

            var invoker = new BindingInvoker(new RuntimeConfiguration(), new Mock<IErrorProvider>().Object);
            TimeSpan duration;
            var result = invoker.InvokeBinding(stepTransformationBinding, contextManagerStub.Object, new object[] { 1234 }, new Mock<ITestTracer>().Object, out duration);
            Assert.NotNull(result);
            Assert.That(result.GetType(), Is.EqualTo(typeof(BankAccount)));
            Assert.That(((BankAccount)result).AccountNumber, Is.EqualTo(1234));
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseStepArgumentTransformationForConversion()
        {
            var stepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateUser");
            TimeSpan duration;
            var resultUser = new User();
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.Convert("user xyz", typeof(User), new CultureInfo("en-US"));
            Assert.That(result, Is.EqualTo(resultUser));
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationForStringConversionIfRegexDoesNotMatch()
        {
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.CanConvert("xyz", new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationForStringConversionIfRegexMatchesButArgumentsCannotBeConvertedToParameterTypes()
        {
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.CanConvert("user xyz with account abcd", new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.False);
        }

        [Test]
        public void StepArgumentTypeConverterShouldUseStepArgumentTransformationForStringConversionIfRegexMatchesAndArgumentsCanBeConvertedToParameterTypes()
        {
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var result = stepArgumentTypeConverter.CanConvert("user xyz with account 1234", new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.True);
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationIfArgumentsCannotBeConvertedToParameterTypes()
        {
            var stepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateUser");
            TimeSpan duration;
            var resultUser = new User();
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), It.IsAny<object[]>(), It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);

            var table = new Table("Name");
            table.AddRow("xyz");
            var result = stepArgumentTypeConverter.Convert(table, new RuntimeBindingType(typeof(User)), new CultureInfo("en-US"));
            Assert.That(result, Is.Not.EqualTo(resultUser));
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationIfArgumentsCanBeConvertedToParameterTypes()
        {
            var table = new Table("Name");

            var stepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateUsers");
            TimeSpan duration;
            var resultUsers = new User[3];
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(stepTransformationBinding, It.IsAny<IContextManager>(), new object[] { table }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUsers);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);
            var result = stepArgumentTypeConverter.Convert(table, typeof(IEnumerable<User>), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultUsers));
        }

        [Test]
        public void StepArgumentTypeConverterShouldNotUseStepArgumentTransformationToConvertArgumentsOfOtherStepArgumentTransformation()
        {
            var bankAccountStepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateBankAccount");
            var userStepTransformationBinding = stepTransformations.Single(x => x.Method.Name == "CreateUserWithBankAccount");

            var resultBankAccount = new BankAccount { AccountNumber = 1234 };
            var resultUser = new User { Name = "xyz", BankAccount = resultBankAccount };

            TimeSpan duration;
            var methodBindingInvokerStub = new Mock<IBindingInvoker>();
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(bankAccountStepTransformationBinding, It.IsAny<IContextManager>(), new object[] { 1234 }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultBankAccount);
            methodBindingInvokerStub.Setup(i => i.InvokeBinding(userStepTransformationBinding, It.IsAny<IContextManager>(), new object[] { "xyz", resultBankAccount }, It.IsAny<ITestTracer>(), out duration))
                .Returns(resultUser);

            var stepArgumentTypeConverter = CreateStepArgumentTypeConverter(methodBindingInvokerStub.Object);
            var result = stepArgumentTypeConverter.Convert("user xyz with account 1234", typeof(User), new CultureInfo("en-US"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultUser));
        }
    }
}