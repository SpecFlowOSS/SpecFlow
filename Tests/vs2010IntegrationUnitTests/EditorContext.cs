using System;
using System.IO;
using System.Text;

namespace Vs2010IntegrationUnitTests
{
    //class EditorContext
    //{
    //    private readonly string input;

    //    public EditorContext(string input, CaretPosition caretPosition)
    //    {
    //        this.input = input;
    //        CaretPosition = caretPosition;
    //    }

    //    public CaretPosition CaretPosition { get; private set; }

    //    public string GetAllText()
    //    {
    //        return input;
    //    }

    //    public int GetAbsoluteCaretPosition()
    //    {
    //        return GetAbsolutePositionFromLine(CaretPosition.LineNumber) + CaretPosition.PositionInLine;
    //    }

    //    private TResult WithReader<TResult>(Func<StreamReader, TResult> action)
    //    {
    //        using (var memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(input)))
    //        {
    //            using (var reader = new StreamReader(memoryStream))
    //            {
    //                return action(reader);
    //            }
    //        }
    //    }

    //    public int GetLineNumberFromPosition(int position)
    //    {
    //        return WithReader(reader =>
    //        {
    //            var line = -1;
    //            while (reader.BaseStream.Position < position && !reader.EndOfStream)
    //            {
    //                var temp = reader.ReadLine();
    //                Console.WriteLine(temp);
    //                line++;
    //            }

    //            return line;
    //        });

    //        //using (var memoryStream = new MemoryStream(new UnicodeEncoding().GetBytes(input)))
    //        //{
    //        //    using (var reader = new StreamReader(memoryStream))
    //        //    {
    //        //        var line = -1;
    //        //        while (memoryStream.Position < position && !reader.EndOfStream)
    //        //        {
    //        //            reader.ReadLine();
    //        //            line++;
    //        //        }

    //        //        return line;
    //        //    }
    //        //}
    //    }

    //    public string GetLineText(int lineNumber)
    //    {
    //        return WithReader(reader =>
    //        {
    //            var currentLineNumber = 0;
    //            string lastLine = null;
    //            while (currentLineNumber <= lineNumber)
    //            {
    //                lastLine = reader.ReadLine();
    //                currentLineNumber++;
    //            }

    //            return lastLine;
    //        });
    //        //using (var memoryStream = new MemoryStream(new UnicodeEncoding().GetBytes(input)))
    //        //{
    //        //    using (var reader = new StreamReader(memoryStream))
    //        //    {
    //        //        var currentLineNumber = 0;
    //        //        string lastLine = null;
    //        //        while (currentLineNumber <= lineNumber)
    //        //        {
    //        //            lastLine = reader.ReadLine();
    //        //            currentLineNumber++;
    //        //        }

    //        //        return lastLine;
    //        //    }
    //        //}
    //    }

    //    public int GetAbsolutePositionFromLine(int lineNumber)
    //    {
    //        return WithReader(reader =>
    //        {
    //            for (var i = 0; i < CaretPosition.LineNumber; i++)
    //            {
    //                reader.ReadLine();
    //            }
    //            return (int)reader.BaseStream.Position;
    //        });
    //        //using (var memoryStream = new MemoryStream(new UnicodeEncoding().GetBytes(input)))
    //        //{
    //        //    using (var reader = new StreamReader(memoryStream))
    //        //    {
    //        //        for (int i = 0; i < CaretPosition.LineNumber; i++)
    //        //        {
    //        //            reader.ReadLine();
    //        //        }
    //        //        return (int) memoryStream.Position;
    //        //    }
    //        //}
    //    }

    //    public int CountLines()
    //    {
    //        return WithReader(reader =>
    //        {
    //            var lines = 0;
    //            while (!reader.EndOfStream)
    //            {
    //                var line = reader.ReadLine();
    //                Console.WriteLine(line);
    //                lines++;
    //            }

    //            return lines;
    //        });
    //    }
    //}
}
