using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class FilePosition
    {
        [XmlAttribute("line")]
        public int Line;
        [XmlAttribute("column")]
        public int Column;

        public FilePosition()
        {
        }

        public FilePosition(int line, int column)
        {
            Line = line;
            Column = column; // TODO: 20100430,jb: remove column since it is not provided by gherkin...
        }
    }
}