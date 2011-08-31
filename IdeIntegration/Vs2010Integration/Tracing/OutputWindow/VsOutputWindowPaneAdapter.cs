using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow
{
    internal sealed class VsOutputWindowPaneAdapter : IOutputWindowPane
    {
        private IVsOutputWindowPane pane;

        public VsOutputWindowPaneAdapter(IVsOutputWindowPane pane)
        {
            if (pane == null) throw new ArgumentNullException("pane");
            this.pane = pane;
        }

        public string Name
        {
            get
            {
                string name = null;
                ErrorHandler.ThrowOnFailure(this.pane.GetName(ref name));
                return name;
            }
            set
            {
                ErrorHandler.ThrowOnFailure(this.pane.SetName(value));
            }
        }

        public void Dispose()
        {
            pane = null;
        }

        public void Activate()
        {
            ErrorHandler.ThrowOnFailure(this.pane.Activate());
        }

        public void Hide()
        {
            ErrorHandler.ThrowOnFailure(this.pane.Hide());
        }

        public void Clear()
        {
            ErrorHandler.ThrowOnFailure(this.pane.Clear());
        }

        public void Write(string text)
        {
            if (text == null) throw new ArgumentNullException("text");

            ErrorHandler.ThrowOnFailure(this.pane.OutputStringThreadSafe(text));
        }

        public void WriteLine(string text)
        {
            if (text == null) throw new ArgumentNullException("text");

            if (!text.EndsWith(Environment.NewLine))
                text += Environment.NewLine;

            Write(text);
        }
    }
}