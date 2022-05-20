using System;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.CucumberExpressions;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings.CucumberExpressions;

public class CucumberExpressionParameterTypeRegistryTests
{
    private BindingRegistry _bindingRegistry;
    private CucumberExpressionParameterTypeRegistry CreateSut()
    {
        _bindingRegistry = new BindingRegistry();
        return new CucumberExpressionParameterTypeRegistry(_bindingRegistry);
    }

    [Fact]
    public void Should_provide_string_type()
    {
        var sut = CreateSut();
        var paramType = sut.LookupByTypeName("string");

        paramType.Should().NotBeNull();
        paramType.RegexStrings.Should().HaveCount(1);//TODO[cukeex]: is this really what we want?
    }
}