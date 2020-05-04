using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Utils;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class FileFilterTests
    {
        private readonly List<string> _validFeatureFilePaths;

        public FileFilterTests()
        {
            _validFeatureFilePaths = new List<string>()
            {
                @"Features\SpecFlowFeature.feature",
                @"Features\Math.feature",
                @"Features\見.feature"
            };
        }

        [Fact]
        public void ShouldReturnValidFilePaths()
        {
            var validatedFilePaths = FileFilter.GetValidFiles(_validFeatureFilePaths);
            validatedFilePaths.Should().BeEquivalentTo(_validFeatureFilePaths);
        }

        [Fact]
        public void ShouldRemoveInvalidFilePaths()
        {
            var notValidFeatureFilePaths = new List<string>()
            {
                @"Features\SpecFlowFeature*.feature",
                @"Features\Math?.feature",
                @"Features\Project|Impossible.feature"
            };

            var featureFilePaths = _validFeatureFilePaths.Concat(notValidFeatureFilePaths).ToList();

            var validatedPaths = FileFilter.GetValidFiles(featureFilePaths);
            validatedPaths.Should().BeEquivalentTo(_validFeatureFilePaths);
        }

        [Fact]
        public void ShouldRemoveWildChars()
        {
            var wildCards = new List<string>()
            {
                @"**\*.feature"
            };

            var featureFilePaths = _validFeatureFilePaths.Concat(wildCards).ToList();
            var validatedPaths = FileFilter.GetValidFiles(featureFilePaths);
            validatedPaths.Should().BeEquivalentTo(_validFeatureFilePaths);
        }

        [Fact]
        public void ShouldBeAnEmptyListIfOnlyWildCards()
        {
            var wildCards = new List<string>()
            {
                @"**\*.feature"
            };

            var validatedPaths = FileFilter.GetValidFiles(wildCards);
            validatedPaths.Should().BeEmpty();
        }
    }
}
