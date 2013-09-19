using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    internal enum CommentUncommentAction
    {
        Comment,
        Uncomment,
        Toggle
    }

    internal class CommentUncommentCommand
    {
        internal static bool IsComment(string text)
        {
            var trimmedLine = text.TrimStart();

            // TODO: Where is a good place to store the comment character as a const?  GherkinLanguageService?
            return trimmedLine.StartsWith("#");
        }

        /// <summary>
        /// Handle the Comment Selection and Uncomment Selection editor commands.  These commands comment/uncomment
        /// the currently selected lines.
        /// </summary>
        /// <param name="editorContext">The editor context</param>
        /// <param name="action">The requested command is Comment Selection, otherwise Uncomment Selection</param>
        /// <returns>True if the operation succeeds to comment/uncomment the selected lines, false otherwise</returns>
        public bool CommentOrUncommentSelection(GherkinEditorContext editorContext, CommentUncommentAction action)
        {
            var selectionStartLine = editorContext.TextView.Selection.Start.Position.GetContainingLine();
            var selectionEndLine = GetSelectionEndLine(selectionStartLine, editorContext.TextView);

            switch (action)
            {
                case CommentUncommentAction.Comment:
                    CommentSelection(selectionStartLine, selectionEndLine);
                    break;
                case CommentUncommentAction.Uncomment:
                    UncommentSelection(selectionStartLine, selectionEndLine);
                    break;
                case CommentUncommentAction.Toggle:
                    if (IsCommented(selectionStartLine))
                        UncommentSelection(selectionStartLine, selectionEndLine);
                    else
                        CommentSelection(selectionStartLine, selectionEndLine);
                    break;
            }

            // Select the entirety of the lines that were just commented or uncommented, have to update start/end lines due to snapshot changes
            selectionStartLine = editorContext.TextView.Selection.Start.Position.GetContainingLine();
            selectionEndLine = GetSelectionEndLine(selectionStartLine, editorContext.TextView);
            editorContext.TextView.Selection.Select(new SnapshotSpan(selectionStartLine.Start, selectionEndLine.End), false);

            return true;
        }

        private bool IsCommented(ITextSnapshotLine line)
        {
            return line.GetText().TrimStart().StartsWith("#");
        }

        private ITextSnapshotLine GetSelectionEndLine(ITextSnapshotLine selectionStartLine, IWpfTextView textView)
        {
            var selectionEndLine = textView.Selection.End.Position.GetContainingLine();
            // if the selection ends exactly at the beginning of a new line (ie line select), we do not comment out the last line
            if (selectionStartLine.LineNumber != selectionEndLine.LineNumber && selectionEndLine.Start.Equals(textView.Selection.End.Position))
            {
                selectionEndLine = selectionEndLine.Snapshot.GetLineFromLineNumber(selectionEndLine.LineNumber - 1);
            }
            return selectionEndLine;
        }

        private void UncommentSelection(ITextSnapshotLine startLine, ITextSnapshotLine endLine)
        {
            using (var textEdit = startLine.Snapshot.TextBuffer.CreateEdit())
            {
                for (int i = startLine.LineNumber; i <= endLine.LineNumber; i++)
                {
                    var curLine = startLine.Snapshot.GetLineFromLineNumber(i);
                    string curLineText = curLine.GetTextIncludingLineBreak();
                    if(IsComment(curLineText))
                    {
                        int commentCharPosition = curLineText.IndexOf('#');
                        textEdit.Delete(curLine.Start.Position + commentCharPosition, 1);
                    }
                }

                textEdit.Apply();
            }
        }

        private void CommentSelection(ITextSnapshotLine startLine, ITextSnapshotLine endLine)
        {
            List<ITextSnapshotLine> lines = new List<ITextSnapshotLine>();
            int commentCharPosition = int.MaxValue;

            // Build up the line collection and determine the position that the comment char will be inserted into
            for (int i = startLine.LineNumber; i <= endLine.LineNumber; i++)
            {
                var curLine = startLine.Snapshot.GetLineFromLineNumber(i);
                string curLineText = curLine.GetText();
                int firstCharPosition = curLineText.Length - curLineText.TrimStart().Length;
                if (firstCharPosition < commentCharPosition)
                {
                    commentCharPosition = firstCharPosition;
                }

                lines.Add(curLine);
            }

            // Add the comment char to each line at commentCharPosition
            using (var textEdit = startLine.Snapshot.TextBuffer.CreateEdit())
            {
                foreach (var line in lines)
                {
                    textEdit.Insert(line.Start.Position + commentCharPosition, "#");
                }

                textEdit.Apply();
            }
        }
    }
}