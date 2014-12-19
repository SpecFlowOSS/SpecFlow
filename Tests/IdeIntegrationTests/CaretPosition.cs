using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeIntegrationTests
{
    class CaretPosition
    {
        private int line;
        private int positionInLine;

        public CaretPosition(int line, int positionInLine)
        {
            this.line = line;
            this.positionInLine = positionInLine;
        }
    }
}
