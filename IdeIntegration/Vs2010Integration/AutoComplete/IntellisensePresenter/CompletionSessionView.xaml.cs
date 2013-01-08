using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete.IntellisensePresenter
{
    public partial class CompletionSessionView
    {
        private readonly CompletionSessionPresenter presenter;

        internal CompletionSessionView(CompletionSessionPresenter presenter)
        {
            InitializeComponent();

            this.presenter = presenter;

            SubscribeToEvents();
            this.DataContext = presenter;
        }

        private void SubscribeToEvents()
        {
            this.presenter.Session.Dismissed += OnSessionDismissed;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            UnsubscribeFromEvents();
            SurrenderFocus();
        }

        private void UnsubscribeFromEvents()
        {
            this.presenter.Session.Dismissed -= OnSessionDismissed;
        }

        private void SurrenderFocus()
        {
            IWpfTextView view = this.presenter.Session.TextView as IWpfTextView;
            if (view != null)
            {
                Keyboard.Focus(view.VisualElement);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.presenter.SelectedCompletion = this.listViewCompletions.SelectedItem as Completion;
            this.SurrenderFocus();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.presenter.Commit();
        }

        internal void Select(Completion completion)
        {
            this.listViewCompletions.SelectedItem = completion;
            if (completion != null)
            {
                this.listViewCompletions.ScrollIntoView(completion);
            }
        }

        private void listViewCompletions_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SurrenderFocus();
        }
    }
}