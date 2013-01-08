using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Vs2010Integration.AutoComplete.IntellisensePresenter;

namespace Microsoft.VisualStudio.IntellisensePresenter
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CompletionSessionView : UserControl
    {
        private CompletionSessionPresenter presenter;

        internal CompletionSessionView(CompletionSessionPresenter presenter)
        {
            InitializeComponent();

            this.presenter = presenter;

            SubscribeToEvents();
            this.DataContext = presenter;
        }

        private void SubscribeToEvents()
        {
            this.presenter.Session.Dismissed += new EventHandler(OnSessionDismissed);
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            UnsubscribeFromEvents();
            SurrenderFocus();
        }

        private void UnsubscribeFromEvents()
        {
            this.presenter.Session.Dismissed -= new EventHandler(OnSessionDismissed);
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
            //this.listViewCompletions.Focus();
            if (completion != null)
            {
                this.listViewCompletions.ScrollIntoView(completion);
            }
        }

        private void listViewCompletions_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.SurrenderFocus();
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double heightAdjust = this.Height + e.VerticalChange;
            if (heightAdjust >= this.MinHeight)
            {
                this.Height = heightAdjust;
            }

            double widthAdjust = this.Width + e.HorizontalChange;
            if (widthAdjust >= this.MinWidth)
            {
                this.Width = widthAdjust;
            }
        }
    }
}