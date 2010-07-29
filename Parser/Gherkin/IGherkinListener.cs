using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gherkin;
using java.util;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public interface IGherkinListener
    {
    }

    internal class ListenerExtender : Listener
    {
        private IGherkinListener gherkinListener;

        public int LineOffset { get; set; }

        private int GetEditorLine(int line)
        {
            return line - 1 + LineOffset;
        }

        public void tag(string name, int line)
        {
        }

        public void comment(string comment, int line)
        {
        }

        public void location(string uri, int offset)
        {
        }

        public void feature(string keyword, string name, string description, int line)
        {
        }

        public void background(string keyword, string name, string description, int line)
        {
        }

        public void scenario(string keyword, string name, string description, int line)
        {
        }

        public void scenarioOutline(string keyword, string name, string description, int line)
        {
        }

        public void examples(string keyword, string name, string description, int line)
        {
        }

        public void step(string keyword, string text, int line)
        {
        }

        public void row(List list, int line)
        {
        }

        public void pyString(string content, int line)
        {
        }

        public void eof()
        {
        }

        public void syntaxError(string state, string eventName, List legalEvents, int line)
        {
        }
        
    }
}
