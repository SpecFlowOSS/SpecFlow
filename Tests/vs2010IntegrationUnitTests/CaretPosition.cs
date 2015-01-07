namespace Vs2010IntegrationUnitTests
{
    class CaretPosition
    {
        public CaretPosition(int line, int positionInLine)
        {
            LineNumber = line;
            PositionInLine = positionInLine;
        }

        public int LineNumber { get; private set; }
        public int PositionInLine { get; private set; }
    }
}
