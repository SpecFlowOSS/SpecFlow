using System;
using System.Collections.Specialized;
using System.Globalization;
using FluentAssertions;
using TechTalk.SpecFlow.Bindings;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Bindings;

public class StepContextTests
{
    [Fact]
    public void Should_Tags_contain_combined_tags_if_ScenarioInfo_provided()
    {
        var featureTags = new[] { "not_used_feature_tag" };
        var directScenarioTags = new[] { "scenario_tag" };
        var inheritedScenarioTags = new[] { "feature_tag", "rule_tag" };
        var sut = new StepContext(CreateFeatureInfo(featureTags), CreateScenarioInfo(directScenarioTags, inheritedScenarioTags));

        sut.Tags.Should().BeEquivalentTo("feature_tag", "rule_tag", "scenario_tag");
    }

    [Fact]
    public void Should_Tags_contain_feature_tags_if_ScenarioInfo_not_provided()
    {
        var featureTags = new[] { "feature_tag" };
        var sut = new StepContext(CreateFeatureInfo(featureTags), null);

        sut.Tags.Should().BeEquivalentTo("feature_tag");
    }

    [Fact]
    public void Should_Tags_be_empty_if_no_info_provided()
    {
        var sut = new StepContext(null, null);

        sut.Tags.Should().BeEmpty();
    }

    private ScenarioInfo CreateScenarioInfo(string[] directScenarioTags = null, string[] inheritedScenarioTags = null)
        => new("Sample scenario", null, directScenarioTags ?? Array.Empty<string>(), new OrderedDictionary(), inheritedScenarioTags ?? Array.Empty<string>());

    private FeatureInfo CreateFeatureInfo(string[] featureTags = null) => 
        new(new CultureInfo("en-US"), @"C:\MyProject", "Sample feature", null, ProgrammingLanguage.CSharp, featureTags ?? Array.Empty<string>());
}