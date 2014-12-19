using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeIntegrationTests
{
    class EditorContext
    {
        private string input;
        private CaretPosition caretPosition;

        public EditorContext(string input, CaretPosition caretPosition)
        {
            this.input = input;
            this.caretPosition = caretPosition;
        }
    }
}
