using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Generator
{
    internal class IndentProcessingWriter : TextWriter
    {
        TextWriter innerWriter;
        private bool trimSpaces = false;

        public IndentProcessingWriter(TextWriter innerWriter)
        {
            this.innerWriter = innerWriter;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }

        public override void Write(char value)
        {
            Write(value.ToString());
        }

        public override void Write(string value)
        {
            if (trimSpaces)
            {
                value = value.TrimStart(' ', '\t');
                if (value == String.Empty)
                    return;
                trimSpaces = false;
            }

            innerWriter.Write(value);
        }

        public override Encoding Encoding
        {
            get { return innerWriter.Encoding; }
        }

        static private readonly Regex indentNextRe = new Regex(@"^[\s\/\']*#indentnext (?<ind>\d+)\s*$");

        public override void WriteLine(string text)
        {
            var match = indentNextRe.Match(text);
            if (match.Success)
            {
                Write(new string(' ', Int32.Parse(match.Groups["ind"].Value)));
                trimSpaces = true;
                return;
            }

            base.WriteLine(text);
        }

        public override string ToString()
        {
            return innerWriter.ToString();
        }

        public override void Flush()
        {
            innerWriter.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                innerWriter.Dispose();
        }
    }
}