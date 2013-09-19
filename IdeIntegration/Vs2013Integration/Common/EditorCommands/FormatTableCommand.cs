using System;
using System.Text;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    internal class FormatTableCommand
    {
        public bool FormatTable(GherkinEditorContext editorContext)
        {
            if (!editorContext.LanguageService.ProjectScope.IntegrationOptionsProvider.GetOptions().EnableTableAutoFormat)
                return false;

            var fileScope = editorContext.LanguageService.GetFileScope();
            if (fileScope == null)
                return false;

            SnapshotPoint caret = editorContext.TextView.Caret.Position.BufferPosition;
            var triggerLineNumber = caret.GetContainingLine().LineNumber;
            var stepBlock = fileScope.GetStepBlockFromStepPosition(triggerLineNumber);
            var step = stepBlock.Steps.LastOrDefault(s => s.BlockRelativeLine + stepBlock.KeywordLine < triggerLineNumber);
            if (step == null)
                return false;

            var stepArgStartPoint = editorContext.TextView.TextSnapshot.GetLineFromLineNumber(stepBlock.KeywordLine + step.BlockRelativeLine + 1).Start;
            var stepArgsEndPoint = caret.GetContainingLine().End;

            // Determine what line the table starts on
            var startLine = caret.GetContainingLine().LineNumber;
            var previousLineText = caret.Snapshot.GetLineFromLineNumber(startLine - 1).GetText();
            while (startLine > 0 && (IsTable(previousLineText) || CommentUncommentCommand.IsComment(previousLineText)))
            {
                previousLineText = caret.Snapshot.GetLineFromLineNumber(--startLine - 1).GetText();
            }

            var start = caret.Snapshot.GetLineFromLineNumber(startLine).Start;
            var span = new SnapshotSpan(start, caret);
            string oldTable = span.GetText();
            string formattedTable = FormatTableString(oldTable);
            if (formattedTable == null || formattedTable.Equals(oldTable))
                return false;
            var textEdit = span.Snapshot.TextBuffer.CreateEdit();
            textEdit.Replace(span, formattedTable);
            textEdit.Apply();
            return true;
        }

        private bool IsTable(string text)
        {
            var trimmedLine = text.TrimStart();
            return trimmedLine.StartsWith("|");
        }

        private string FormatTableString(string oldTable)
        {
            const string escapedPipeString = "\\\0";
            oldTable = oldTable.Replace("\\|", escapedPipeString);

            string[] lines = oldTable.Replace("\r\n", "\n").Split('\n');
            if (lines.Length < 2)
                return null;

            int colCount = lines.Max(l => l.Count(c => c == '|')) - 1;
            if (colCount == 0)
                return null;
            int[] widths = new int[colCount];

            string indent = null;
            foreach (var line in lines)
            {
                if (line.Count(c => c == '|') < 2)
                    continue;

                if (indent == null)
                    indent = line.Substring(0, line.IndexOf('|'));

                var cells = GetCells(line);
                for (int i = 0; i < Math.Min(cells.Length, widths.Length); i++)
                {
                    int cellLength = cells[i].Trim().Length;
                    widths[i] = Math.Max(widths[i], cellLength);
                }
            }
            const string padding = " ";
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.Count(c => c == '|') < 2)
                {
                    stringBuilder.AppendLine(line);
                    continue;
                }

                var cells = GetCells(line);
                stringBuilder.Append(indent);
                stringBuilder.Append('|');
                for (int i = 0; i < cells.Length; i++)
                {
                    stringBuilder.Append(padding);
                    var trimmedCell = cells[i].Trim();
                    stringBuilder.Append(trimmedCell);
                    if (i < widths.Length)
                        stringBuilder.Append(' ', widths[i] - trimmedCell.Length);
                    stringBuilder.Append(padding);
                    stringBuilder.Append('|');
                }
                stringBuilder.AppendLine();
            }

            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return stringBuilder.ToString().Replace(escapedPipeString, "\\|");
        }

        private string[] GetCells(string line)
        {
            line = line.Trim();
            if (line.StartsWith("|"))
                line = line.Substring(1);
            if (line.EndsWith("|"))
                line = line.Substring(0, line.Length - 1);
            return line.Split('|');
        }
    }
}