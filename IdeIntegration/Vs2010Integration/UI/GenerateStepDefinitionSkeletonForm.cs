using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Commands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.UI
{
    public partial class GenerateStepDefinitionSkeletonForm : Form
    {
        private readonly DTE dte;
        public Action<GenerateStepDefinitionSkeletonForm> OnPreview { get; set; }
        public Func<GenerateStepDefinitionSkeletonForm, bool> OnCopy { get; set; }
        public Func<GenerateStepDefinitionSkeletonForm, string, bool> OnGenerate { get; set; }

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

        private class StyleItem
        {
            public StepDefinitionSkeletonStyle Style { get; private set; }

            public StyleItem(StepDefinitionSkeletonStyle style)
            {
                Style = style;
            }

            public override string ToString()
            {
                return ((DescriptionAttribute)Attribute.GetCustomAttribute(typeof(StepDefinitionSkeletonStyle).GetField(Style.ToString()), typeof(DescriptionAttribute))).Description;
            }
        }

        private readonly string defaultFolder;

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

        public GenerateStepDefinitionSkeletonForm(string featureTitle, StepInstance[] steps, Project specFlowProject, StepDefinitionSkeletonStyle stepDefinitionSkeletonStyle, ProgrammingLanguage defaultLanguage, DTE dte)
        {
            this.dte = dte;
            InitializeComponent();

            stepsList.BeginUpdate();
            stepsList.Items.Clear();
            foreach (var step in steps)
            {
                stepsList.Items.Add(new ListItem(step), true);
            }
            stepsList.EndUpdate();

            classNameTextBox.Text = string.Format("{0} Steps", featureTitle).ToIdentifier();

            styleComboBox.BeginUpdate();
            var styles = Enum.GetValues(typeof (StepDefinitionSkeletonStyle)).Cast<StepDefinitionSkeletonStyle>()
                .Where(value => value != StepDefinitionSkeletonStyle.MethodNameRegex || defaultLanguage == ProgrammingLanguage.FSharp)
                .ToArray();
            styleComboBox.Items.Clear();
            styleComboBox.Items.AddRange(styles.Select(s => new StyleItem(s)).ToArray<object>());

            int selectedIndex = Array.IndexOf(styles, stepDefinitionSkeletonStyle);
            styleComboBox.SelectedIndex = selectedIndex < 0 ? 0 : selectedIndex;
            styleComboBox.EndUpdate();

            defaultFolder = Path.Combine(VsxHelper.GetProjectFolder(specFlowProject), "StepDefinitions");
            if (!Directory.Exists(defaultFolder))
                defaultFolder = VsxHelper.GetProjectFolder(specFlowProject);
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

        private void previewButton_Click(object sender, EventArgs e)
        {
            OnPreview(this);
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            if (OnCopy(this))
            {
                DialogResult = DialogResult.No;
            }
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = Path.Combine(defaultFolder, ClassName);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (OnGenerate(this, saveFileDialog.FileName))
                {
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void helpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = (string)((LinkLabel)sender).Tag;
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to open the browser.");
            }
        }
    }
}
