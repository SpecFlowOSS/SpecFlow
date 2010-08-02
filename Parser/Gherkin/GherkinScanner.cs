using System;
using System.Text.RegularExpressions;
using gherkin;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public class GherkinScanner
    {
        private readonly I18n languageService;
        private readonly GherkinBuffer buffer;

        public GherkinScanner(I18n languageService, string gherkinText)
            : this(languageService, gherkinText, 0)
        {
        }

        public GherkinScanner(I18n languageService, string gherkinText, int lineOffset)
        {
            this.languageService = languageService;
            this.buffer = new GherkinBuffer(gherkinText, lineOffset);
        }

        public void Scan(IGherkinListener listener)
        {
            ListenerExtender listenerExtender = new ListenerExtender(languageService, listener, buffer);
            DoScan(listenerExtender, buffer.LineOffset, 0);
        }

        private void DoScan(ListenerExtender listenerExtender, int startLine, int errorRertyCount)
        {
            const int MAX_ERROR_RETRY = 5;
            const int SKIP_LINES_BEFORE_RETRY = 1;

            listenerExtender.LineOffset = startLine;
            var contentToScan = buffer.GetContentFrom(startLine);

            try
            {
                Lexer lexer = languageService.lexer(listenerExtender);
                lexer.scan(contentToScan, null, 0);
            }
            catch(ScanningCancelledException)
            {
                throw;
            }
            catch (LexingError lexingError)
            {
                int? errorLine = GetErrorLine(lexingError, listenerExtender.LineOffset);

                RegisterError(lexingError, errorLine, listenerExtender.GherkinListener);

                if (errorLine != null &&
                    errorLine.Value + SKIP_LINES_BEFORE_RETRY <= buffer.LineCount - 1 &&
                    errorRertyCount < MAX_ERROR_RETRY)
                {
                    var restartLineNumber = errorLine.Value + SKIP_LINES_BEFORE_RETRY;
                    
                    DoScan(
                        listenerExtender, 
                        restartLineNumber, 
                        errorRertyCount + 1);
                }
            }
            catch(Exception ex)
            {
                RegisterError(ex, listenerExtender.GherkinListener);
            }
        }

        private void RegisterError(Exception exception, IGherkinListener gherkinListener)
        {
            gherkinListener.Error(exception.Message, buffer.EndPosition, exception);
        }

        private void RegisterError(LexingError lexingError, int? errorLine, IGherkinListener gherkinListener)
        {
            string message = GetErrorMessage(lexingError);
            GherkinBufferPosition errorPosition = errorLine == null
                                                      ? buffer.EndPosition
                                                      : buffer.GetLineStartPosition(errorLine.Value);

            gherkinListener.Error(message, errorPosition, lexingError);
        }

        static private readonly Regex lineNoRe = new Regex(@"^Lexing error on line (?<lineno>\d+):");
        private int? GetErrorLine(LexingError lexingError, int lineOffset)
        {
            var match = lineNoRe.Match(lexingError.Message);
            if (!match.Success)
                return null;

            int parserdLine = Int32.Parse(match.Groups["lineno"].Value);
            return parserdLine - 1 + lineOffset;
        }

        private string GetErrorMessage(LexingError lexingError)
        {
            var match = lineNoRe.Match(lexingError.Message);
            if (!match.Success)
                return lexingError.Message;

            return lexingError.Message.Substring(match.Length).Trim();
        }
    }
}