using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Parser_handles_empty_feature_file_without_error()
        {
            var parser = new SpecFlowGherkinParser(CultureInfo.GetCultureInfo("en"));

            Action act = () => parser.Parse(new StringReader(""), null);

            act.ShouldNotThrow();
        }
    }
}
