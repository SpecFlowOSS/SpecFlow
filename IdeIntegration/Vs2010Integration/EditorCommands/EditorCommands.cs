using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    internal class EditorCommands
    {
        private readonly GherkinLanguageService languageService;
        private readonly IWpfTextView textView;

        public EditorCommands(GherkinLanguageService languageService, IWpfTextView textView)
        {
            this.languageService = languageService;
            this.textView = textView;
        }

        private IBindingMatchService GetBindingMatchService()
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            return bindingMatchService;
        }

        private GherkinStep GetCurrentStep()
        {
            var fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return null;

            SnapshotPoint caret = textView.Caret.Position.BufferPosition;
            return fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber);
        }

        public bool CanGoToDefinition()
        {
            return GetBindingMatchService() != null && GetCurrentStep() != null;
        }

        public bool GoToDefinition()
        {
            var step = GetCurrentStep();
            if (step == null)
                return false;

            var bindingMatchService = GetBindingMatchService();
            if (bindingMatchService == null)
                return false;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
                return true;
            }

            IEnumerable<StepBinding> candidatingBindings;
            var binding = bindingMatchService.GetBestMatchingBinding(step, out candidatingBindings);

            if (binding == null)
            {
                if (candidatingBindings.Any())
                {
                    string bindingsText = string.Join(Environment.NewLine, candidatingBindings.Select(b => b.Method.ShortDisplayText));
                    MessageBox.Show("Multiple matching bindings found. Navigating to the first match..."
                        + Environment.NewLine + Environment.NewLine + bindingsText, "Go to binding");
                    binding = candidatingBindings.First();
                }
                else
                {
                    string skeleton = languageService.ProjectScope.StepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(step);

                    var result = MessageBox.Show("No matching step binding found for this step! Do you want to copy the step binding skeleton to the clipboard?"
                         + Environment.NewLine + Environment.NewLine + skeleton, "Go to binding", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        Clipboard.SetText(skeleton);
                    }
                    return true;
                }
            }

            var method = binding.Method;
            var codeFunction = new VsStepSuggestionBindingCollector().FindCodeFunction(((VsProjectScope) languageService.ProjectScope), method);

            if (codeFunction != null)
            {
                if (!codeFunction.ProjectItem.IsOpen)
                {
                    codeFunction.ProjectItem.Open();
                }
                var navigatePoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartHeader);
                navigatePoint.TryToShow();
                navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
            }

            return true;
        }

        private GherkinStep GetCurrentStepForAtguments()
        {
            var fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return null;

            SnapshotPoint caret = textView.Caret.Position.BufferPosition;
            var triggerLineNumber = caret.GetContainingLine().LineNumber;
            var stepBlock = fileScope.GetStepBlockFromStepPosition(triggerLineNumber);
            return stepBlock.Steps.LastOrDefault(s => s.BlockRelativeLine + stepBlock.KeywordLine < triggerLineNumber);
        }

        public bool FormatTable()
        {
            if (!languageService.ProjectScope.IntegrationOptionsProvider.GetOptions().EnableTableAutoFormat)
                return false;

            var fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return false;

            SnapshotPoint caret = textView.Caret.Position.BufferPosition;
            var triggerLineNumber = caret.GetContainingLine().LineNumber;
            var stepBlock = fileScope.GetStepBlockFromStepPosition(triggerLineNumber);
            var step = stepBlock.Steps.LastOrDefault(s => s.BlockRelativeLine + stepBlock.KeywordLine < triggerLineNumber);
            if (step == null)
                return false;

            var stepArgStartPoint = textView.TextSnapshot.GetLineFromLineNumber(stepBlock.KeywordLine + step.BlockRelativeLine + 1).Start;
            var stepArgsEndPoint = caret.GetContainingLine().End;

            var startLine = caret.GetContainingLine().LineNumber;
            while (startLine > 0 && IsTableOrComment(caret.Snapshot.GetLineFromLineNumber(startLine - 1).GetText()))
            {
                startLine--;
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

        private bool IsTableOrComment(string text)
        {
            var trimmedLine = text.TrimStart();
            return trimmedLine.StartsWith("#") || trimmedLine.StartsWith("|");
        }

        private string FormatTableString(string oldTable)
        {
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
                for (int i = 0; i < cells.Length; i++)
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
                    stringBuilder.Append(' ', widths[i] - trimmedCell.Length);
                    stringBuilder.Append(padding);
                    stringBuilder.Append('|');
                }
                stringBuilder.AppendLine();
            }

            stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return stringBuilder.ToString();
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