using TechTalk.SpecFlow.Parser.Gherkin;

namespace Vs2010IntegrationUnitTests
{
    static class GherkinBufferExtensions
    {
        public static int GetLineNumberFromPosition(this GherkinBuffer gherkinBuffer, int position)
        {
            var line = 0;
            var currentPosition = 0;
            while(currentPosition < position && line < gherkinBuffer.LineCount - 1)
            {
                line++;
                currentPosition = gherkinBuffer.GetBufferPositionFromLine(line);
            }
            return line;
        }
    }
}
