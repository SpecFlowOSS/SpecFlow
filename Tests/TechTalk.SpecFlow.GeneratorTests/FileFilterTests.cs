using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        [SkippableFact]
        public void ShouldReturnValidFilePaths()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            var validatedFilePaths = FileFilter.GetValidFiles(_validFeatureFilePaths);
            validatedFilePaths.Should().BeEquivalentTo(_validFeatureFilePaths);
        }

        [SkippableFact]
        public void ShouldRemoveInvalidFilePaths()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

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

        [SkippableFact]
        public void ShouldRemoveWildChars()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            var wildCards = new List<string>()
            {
                @"**\*.feature"
            };

            var featureFilePaths = _validFeatureFilePaths.Concat(wildCards).ToList();
            var validatedPaths = FileFilter.GetValidFiles(featureFilePaths);
            validatedPaths.Should().BeEquivalentTo(_validFeatureFilePaths);
        }

        [SkippableFact]
        public void ShouldBeAnEmptyListIfOnlyWildCards()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            var wildCards = new List<string>()
            {
                @"**\*.feature"
            };

            var validatedPaths = FileFilter.GetValidFiles(wildCards);
            validatedPaths.Should().BeEmpty();
        }
    }
}
