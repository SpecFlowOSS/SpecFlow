using System;
using System.Linq;
using System.Windows.Forms;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Commands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.UI
{
    public partial class GenerateStepDefinitionSkeletonForm : Form
    {
        private class ListItem
        {
            private readonly bool fileScopedDisplay;
            public StepInstance Step { get; private set; }

            public ListItem(StepInstance step, bool fileScopedDisplay = false)
            {
                this.fileScopedDisplay = fileScopedDisplay;
                Step = step;
            }

            public override string ToString()
            {
                var label = fileScopedDisplay ? Step.GetFileScopedLabel() : Step.GetLabel();
                return string.Format("[{0}] {1}", Step.StepDefinitionType, label);
            }
        }

        public string ClassName
        {
            get { return classNameTextBox.Text; }
        }

        public StepInstance[] SelectedSteps
        {
            get { return stepsList.CheckedItems.Cast<ListItem>().Select(li => li.Step).ToArray(); }
        }

        public StepDefinitionSkeletonStyle Style
        {
            get { return (StepDefinitionSkeletonStyle)styleComboBox.SelectedIndex; }
        }

        public GenerateStepDefinitionSkeletonForm(string featureTitle, StepInstance[] steps)
        {
            InitializeComponent();

            stepsList.BeginUpdate();
            stepsList.Items.Clear();
            foreach (var step in steps)
            {
                stepsList.Items.Add(new ListItem(step), true);
            }
            stepsList.EndUpdate();

            classNameTextBox.Text = string.Format("{0} Steps", featureTitle).ToIdentifier();

            styleComboBox.SelectedIndex = 0;
        }

        private void selectAllButton_Click(object sender, EventArgs e)
        {
            SetAllSelected(true);
        }

        private void selectNoneButton_Click(object sender, EventArgs e)
        {
            SetAllSelected(false);
        }

        private void SetAllSelected(bool value)
        {
            stepsList.BeginUpdate();
            for (int i = 0; i < stepsList.Items.Count; i++)
                stepsList.SetItemCheckState(i, value ? CheckState.Checked : CheckState.Unchecked);
            stepsList.EndUpdate();
        }
    }
}
