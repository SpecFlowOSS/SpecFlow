using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    public abstract class TaskBase : Task
    {
        public bool ShowTrace { get; set;}

        protected internal CompilerErrorCollection Errors { get; private set; }

        public override bool Execute()
        {
            Errors = new CompilerErrorCollection();
            try
            {
                DoExecute();
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }


            // handle errors
            if (Errors.Count > 0)
            {
                bool hasErrors = false;
                foreach (CompilerError error in Errors)
                {
                    if (error.IsWarning)
                        OutputWarning(error.ToString(), error.ErrorText, error.FileName, error.Line, error.Column);
                    else
                    {
                        OutputError(error.ToString(), error.ErrorText, error.FileName, error.Line, error.Column);
                        hasErrors = true;
                    }
                }

                return !hasErrors;
            }

            return true;
        }

        public void RecordException(Exception ex)
        {
            string message = ex.Message;
            if (ShowTrace)
                message += Environment.NewLine + ex;
            Errors.Add(new CompilerError(String.Empty, 0, 0, null, message));
        }

        public void RecordError(string message, string fileName, int lineNumber, int columnNumber)
        {
            Errors.Add(new CompilerError(fileName, lineNumber, columnNumber, null, message));
        }

        protected void OutputError(string outString, string message, string fileName, int lineNumber, int columnNumber)
        {
            message = message.TrimEnd('\n', '\r');

            Log.LogError(null, null, null, fileName, lineNumber, columnNumber, 0, 0, message);
        }

        protected void OutputWarning(string outString, string message, string fileName, int lineNumber, int columnNumber)
        {
            message = message.TrimEnd('\n', '\r');

            Log.LogWarning(null, null, null, fileName, lineNumber, columnNumber, 0, 0, message);
        }

        protected void OutputInformation(MessageImportance importance, string message, params object[] messageArgs)
        {
            message = message.TrimEnd('\n', '\r');

            Log.LogMessage(importance, message, messageArgs);
        }

        private class MessageTextWriter : TextWriter
        {
            private TaskBase task;
            private MessageImportance importance = MessageImportance.Normal;

            public MessageTextWriter(TaskBase task)
            {
                this.task = task;
            }

            public MessageTextWriter(TaskBase task, MessageImportance importance)
            {
                this.task = task;
                this.importance = importance;
            }

            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }

            public override void Write(char value)
            {
                Write(value.ToString());
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Write(new string(buffer, index, count));
            }

            public override void Write(string value)
            {
                task.OutputInformation(importance, value);
            }
        }

        protected TextWriter GetMessageWriter()
        {
            return new MessageTextWriter(this);
        }

        protected TextWriter GetMessageWriter(MessageImportance importance)
        {
            return new MessageTextWriter(this, importance);
        }

        protected abstract void DoExecute();
    }
}