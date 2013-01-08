using System;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Text.Adornments;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete.IntellisensePresenter
{
    internal class CompletionSessionPresenter : IPopupIntellisensePresenter, IIntellisenseCommandTarget
    {
        private readonly CompletionSessionView view;
        private readonly ICompletionSession session;
        private readonly IServiceProvider serviceProvider;

        internal CompletionSessionPresenter(IServiceProvider serviceProvider, ICompletionSession session)
        {
            this.session = session;
            this.session.SelectedCompletionSet.SelectionStatusChanged += OnSelectedCompletionSetSelectionStatusChanged;

            this.view = new CompletionSessionView(this);
            this.view.Select(this.SelectedCompletion);
            this.serviceProvider = serviceProvider;
        }

        internal void Navigate(string uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                if (serviceProvider != null)
                {
                    DTE vs = serviceProvider.GetService(typeof(DTE)) as DTE;
                    if (vs != null)
                    {
                        vs.ItemOperations.Navigate(uri, vsNavigateOptions.vsNavigateOptionsDefault);
                    }
                }
            }

        }

        private void OnSelectedCompletionSetSelectionStatusChanged(object sender, ValueChangedEventArgs<CompletionSelectionStatus> e)
        {
            if (e.NewValue != null && e.NewValue.Completion != null)
            {
                this.view.Select(e.NewValue.Completion);
            }
        }

        internal Completion SelectedCompletion
        {
            get
            {
                return session.SelectedCompletionSet.SelectionStatus.Completion;
            }
            set
            {
                session.SelectedCompletionSet.SelectionStatus = new CompletionSelectionStatus(value, true, true);
            }
        }

        internal void Commit()
        {
            this.session.Commit();                
        }

        /// <summary>
        /// 
        /// </summary>
        public PopupStyles PopupStyles
        {
            get { return PopupStyles.PositionClosest; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ITrackingSpan PresentationSpan
        {
            get
            {
                SnapshotSpan span = this.session.SelectedCompletionSet.ApplicableTo.GetSpan(this.session.TextView.TextSnapshot);
                NormalizedSnapshotSpanCollection spans = this.session.TextView.BufferGraph.MapUpToBuffer(span, this.session.SelectedCompletionSet.ApplicableTo.TrackingMode, this.session.TextView.TextBuffer);
                if (spans.Count <= 0)
                {
                    throw new InvalidOperationException("Completion Session Applicable-To Span is invalid.  It doesn't map to a span in the session's text view.");
                }
                SnapshotSpan span2 = spans[0];
                return this.session.TextView.TextBuffer.CurrentSnapshot.CreateTrackingSpan(span2.Span, SpanTrackingMode.EdgeInclusive);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SpaceReservationManagerName
        {
            get { return "completion"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UIElement SurfaceElement
        {
            get { return this.view; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IIntellisenseSession Session
        {
            get { return this.session; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Opacity
        {
            get
            {
                return this.view.Opacity;
            }
            set
            {
                this.view.Opacity = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool ExecuteKeyboardCommand(IntellisenseKeyboardCommand command)
        {
            switch (command)
            {
                case IntellisenseKeyboardCommand.Up:
                    SelectCompletion(-1);
                    return true;
                case IntellisenseKeyboardCommand.PageUp:
                    SelectCompletion(-10);
                    return true;
                case IntellisenseKeyboardCommand.Down:
                    SelectCompletion(1);
                    return true;
                case IntellisenseKeyboardCommand.PageDown:
                    SelectCompletion(10);
                    return true;
                case IntellisenseKeyboardCommand.Escape:
                    this.Session.Dismiss();
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 0067
        public event EventHandler PresentationSpanChanged;
#pragma warning restore 0067

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 0067
        public event EventHandler SurfaceElementChanged;
#pragma warning restore 0067

        private void SelectCompletion(int relativeIndex)
        {
            CompletionSet completionSet = this.session.SelectedCompletionSet;

            int selectedCompletionIndex = completionSet.Completions.IndexOf(SelectedCompletion) + relativeIndex;
            if (selectedCompletionIndex < 0)
                selectedCompletionIndex = 0;
            else if (selectedCompletionIndex >= completionSet.Completions.Count)
                selectedCompletionIndex = completionSet.Completions.Count - 1;

            if (selectedCompletionIndex >= 0 && selectedCompletionIndex < completionSet.Completions.Count)
            {
                this.SelectedCompletion = completionSet.Completions[selectedCompletionIndex];
            }
        }


#pragma warning disable 0067
        public event EventHandler<ValueChangedEventArgs<PopupStyles>> PopupStylesChanged;
#pragma warning restore 0067
    }
}