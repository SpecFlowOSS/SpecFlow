﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public abstract class CommandBasedTestRunnerGateway : ITestRunnerGateway
    {
        protected readonly DTE dte;
        protected readonly IIdeTracer tracer;
        private readonly bool delayedCodeBehindClose;

        protected abstract string GetRunInCurrentContextCommand(bool debug);

        protected CommandBasedTestRunnerGateway(DTE dte, IIdeTracer tracer, bool delayedCodeBehindClose = false)
        {
            this.dte = dte;
            this.tracer = tracer;
            this.delayedCodeBehindClose = delayedCodeBehindClose;
        }

        private bool RunFromCodeBehind(ProjectItem projectItem, Func<TextDocument, int> lineProvider, bool debug)
        {
            if (projectItem.IsDirty)
            {
                projectItem.Save();
            }

            var codeBehindItem = GetCodeBehindItem(projectItem);
            if (codeBehindItem == null)
                return false;

            bool wasOpen = codeBehindItem.IsOpen;
            if (!wasOpen)
            {
                codeBehindItem.Open();
            }
            try
            {
                TextDocument codeBehindTextDocument = (TextDocument)codeBehindItem.Document.Object("TextDocument");

                var codeBehindLine = lineProvider(codeBehindTextDocument);
                if (codeBehindLine == 0)
                {
                    // could not find the line, run the entire file
                    codeBehindLine = 1;
                }

                EditPoint navigatePoint = codeBehindTextDocument.StartPoint.CreateEditPoint();
                navigatePoint.MoveToLineAndOffset(codeBehindLine, 1);
                navigatePoint.TryToShow();
                navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);

                return RunInCurrentContext(debug);
            }
            finally
            {
                if (!wasOpen && !codeBehindItem.IsDirty)
                {
                    CloseDocument(codeBehindItem.Document);
                }
                projectItem.Document.Activate();
            }
        }

// ReSharper disable NotAccessedField.Local
        // we keep a reference to the timer, in order to avoid garbage collection
        private Timer documentCloseTimer = null;
// ReSharper restore NotAccessedField.Local

        private Timer CreateTimer(Action action, int msec)
        {
            var timer = new Timer();
            timer.Tick += (sender, args) =>
                {
                    timer.Stop();
                    action();
                };
            timer.Interval = msec;
            timer.Start();
            return timer;
        }

        private void CloseDocument(Document document)
        {
            if (delayedCodeBehindClose)
            {
                documentCloseTimer = CreateTimer(() =>
                    {
                        documentCloseTimer = null;
                        document.Close();
                    }, 500);
            }
            else
            {
                document.Close();
            }
        }

        private bool RunInCurrentContext(bool debug)
        {
            try
            {
                dte.ExecuteCommand(GetRunInCurrentContextCommand(debug));
                return true;
            }
            catch(Exception ex)
            {
                tracer.Trace("test tool error: " + ex, GetType().Name);
                return false;
            }
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope, bool debug)
        {
            int sourceLine = currentScenario.KeywordLine + 1; // keywordline is zero-indexed
            return RunFromCodeBehind(projectItem, codeBehindTextDocument => GetCodeBehindLine(codeBehindTextDocument, sourceLine, currentScenario), debug);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            var fileName = VsxHelper.GetFileName(projectItem);
            if (fileName != null && fileName.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase))
                return RunFromCodeBehind(projectItem, GetFeatureCodeBehindLine, debug);

            return RunInCurrentContext(debug);
        }

        public bool RunFeatures(Project project, bool debug)
        {
            return RunInCurrentContext(debug);
        }

        protected virtual int GetFeatureCodeBehindLine(TextDocument codeBehindDoc)
        {
            return 1;
        }

        private static readonly Regex linePragmaRe = new Regex(@"^((#line\s+(?<lineno>\d+))|(#ExternalSource\("".*"",(?<lineno>\d+)\)))\s*$");

        protected virtual int GetCodeBehindLine(TextDocument codeBehindDoc, int sourceLine, IScenarioBlock currentScenario)
        {
            int result = GetCodeBehindLineFromLinePragmas(codeBehindDoc, sourceLine);
            if (result > 0 || currentScenario == null)
                return result;

            return GetCodeBehindLineFromScenarioTitle(codeBehindDoc, currentScenario);
        }

        private int GetCodeBehindLineFromScenarioTitle(TextDocument codeBehindDoc, IScenarioBlock currentScenario)
        {
            // we look for the following pattern
            // c#: new TechTalk.SpecFlow.ScenarioInfo("Add two numbers... \"\\"
            // VB: New TechTalk.SpecFlow.ScenarioInfo("Add two numbers... ""\"

            var title = currentScenario.Title;

            var csTitleLiteral = "\"" + title.Replace(@"\", @"\\").Replace("\"", "\\\"") + "\"";
            var csPatternToLookFor = string.Format("new TechTalk.SpecFlow.ScenarioInfo({0}", csTitleLiteral);

            var vbTitleLiteral = "\"" + title.Replace("\"", "\"\"") + "\"";
            var vbPatternToLookFor = string.Format("New TechTalk.SpecFlow.ScenarioInfo({0}", vbTitleLiteral);

            return FindPatternInDocument(codeBehindDoc, csPatternToLookFor, vbPatternToLookFor);
        }

        private int FindPatternInDocument(TextDocument codeBehindDoc, params string[] patternsToLookFor)
        {
            EditPoint start = codeBehindDoc.StartPoint.CreateEditPoint();
            int lineCount = codeBehindDoc.EndPoint.Line;
            while (start.Line <= lineCount - 1)
            {
                start.LineDown();
                string lineText = start.GetText(start.LineLength);

                if (patternsToLookFor.Any(lineText.Contains))
                    return start.Line + 1;
            }

            return 0;
        }


        private static int GetCodeBehindLineFromLinePragmas(TextDocument codeBehindDoc, int sourceLine)
        {
            EditPoint start = codeBehindDoc.StartPoint.CreateEditPoint();
            int lineCount = codeBehindDoc.EndPoint.Line;
            while (start.Line <= lineCount - 1)
            {
                start.LineDown();
                string lineText = start.GetText(start.LineLength);
                //#line 8
                //#ExternalSource("SpecFlowFeature2.feature",8)
                var match = linePragmaRe.Match(lineText);
                if (match.Success)
                {
                    int linePragmaValue;
                    if (int.TryParse(match.Groups["lineno"].Value, out linePragmaValue))
                    {
                        if (linePragmaValue >= sourceLine)
                        {
                            return start.Line + 1;
                        }
                    }
                }
            }

            return 0;
        }

        private ProjectItem GetCodeBehindItem(ProjectItem projectItem)
        {
            if (projectItem.ProjectItems == null)
                return null;

            return projectItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault();
        }
    }
}