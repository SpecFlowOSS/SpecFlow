using System.IO;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ReportInfo
    {
        private string content;
        public string Content
        {
            get
            {
                if (content == null)
                {
                    AssertExists();
                    content = File.ReadAllText(FilePath);
                }
                return content;
            }
            set { content = value; }
        }

        public string FilePath { get; set; }

        public void AssertExists()
        {
            File.Exists(FilePath).Should().BeTrue($"result is generated at path {FilePath}");
        }

        public void AssertEqualIgnoringWhitespace(string expectedValue)
        {
            var expectedValueFormatted = NormalizeWhitespace(CleanHtml(expectedValue));
            var actualValueFormatted = NormalizeWhitespace(CleanHtml(Content));

            actualValueFormatted.Should().BeEquivalentTo(expectedValueFormatted);
            
        }

        public void AssertContainsIgnoringWhitespace(string expectedValue)
        {
            var expectedValueFormatted = NormalizeWhitespace(HtmlEncode(expectedValue)).ToLowerInvariant();
            var actualValueFormatted = NormalizeWhitespace(CleanHtml(Content)).ToLowerInvariant();

            expectedValueFormatted.Should().ContainEquivalentOf(actualValueFormatted);
        }

        private string NormalizeWhitespace(string value)
        {
            var whitespaceRe = new Regex(@"\s+");
            return whitespaceRe.Replace(value.Trim(), "");
        }

        private string HtmlEncode(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string CleanHtml(string value)
        {
            var bodyRe = new Regex(@"\<\/?\s*body\s*\>");
            var bodyMatch = bodyRe.Match(value);
            if (bodyMatch.Success)
            {
                value = value.Substring(bodyMatch.Index + bodyMatch.Value.Length);
                bodyMatch = bodyRe.Match(value);
                if (bodyMatch.Success)
                    value = value.Substring(0, bodyMatch.Index);
            }
            var htmlTagRe = new Regex(@"\<.*?\>");
            value = htmlTagRe.Replace(value.Trim(), " ");

            var nbspRe = new Regex(@"\&nbsp;", RegexOptions.IgnoreCase);
            value = nbspRe.Replace(value, " ");

            return value;
        }
    }
}