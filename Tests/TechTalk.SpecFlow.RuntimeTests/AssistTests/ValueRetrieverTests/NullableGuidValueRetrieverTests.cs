using System;
using Xunit;
using FluentAssertions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ValueRetrieverTests
{
    
    public class NullableGuidValueRetrieverTests
    {
        [Fact]
        public void Returns_the_value_from_the_guid_retriever()
        {
            Func<string, Guid> func = v =>
                                          {
                                              if (v == "x") return new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}");
                                              if (v == "y") return new Guid("{08C91240-68EB-4B11-AFCC-95ED7D44EC67}");
                                              return new Guid();
                                          };

            var retriever = new NullableGuidValueRetriever(func);

            retriever.GetValue("x").Should().Be(new Guid("{A235BE7F-AFEE-43E8-BF01-F279A371662B}"));
            retriever.GetValue("y").Should().Be(new Guid("{08C91240-68EB-4B11-AFCC-95ED7D44EC67}"));
        }

        [Fact]
        public void Returns_null_when_passed_null()
        {
            var retriever = new NullableGuidValueRetriever(v => new Guid("{E9BC63B9-508F-44D6-BD56-B10989B9CCDC}"));

            retriever.GetValue(null).Should().Be(null);
        }

        [Fact]
        public void Returns_null_when_passed_empty()
        {
            var retriever = new NullableGuidValueRetriever(v => new Guid("{E9BC63B9-508F-44D6-BD56-B10989B9CCDC}"));

            retriever.GetValue("").Should().Be(null);
        }
    }
}