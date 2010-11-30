using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.Vs2010Integration.Options
{
    /// <summary>
    // Extends a standard dialog functionality for implementing ToolsOptions pages, 
    // with support for the Visual Studio automation model, Windows Forms, and state 
    // persistence through the Visual Studio settings mechanism.
    /// </summary>
    [Guid("D41B81C9-8501-4124-B75F-0F194E37178C")]
    [ComVisible(true)]
    public class OptionsPageGeneral : DialogPage
    {
        #region Fields
        private int optionCustomInteger = 567;
        private Size optionCustomSize = new Size(50, 50);
        private string optionCustomString = "Hello World";
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets the String type custom option value.
        /// </summary>
        /// <remarks>The property that you want to be show in the options page.</remarks>
        [Category("String Options")]
        [Description("My string option")]
        public string OptionString
        {
            get
            {
                return optionCustomString;
            }
            set
            {
                optionCustomString = value;
            }
        }
        /// <summary>
        /// Gets or sets the integer type custom option value.
        /// </summary>
        /// <remarks>The property that you want to be show in the options page.</remarks>
        [Category("Integer Options")]
        [Description("My integer option")]
        public int OptionInteger
        {
            get
            {
                return optionCustomInteger;
            }
            set
            {
                optionCustomInteger = value;
            }
        }
        /// <summary>
        /// Gets or sets the Size type custom option value.
        /// </summary>
        /// <remarks>The property that you want to be show in the options page.</remarks>
        [Category("Expandable Options")]
        [Description("My Expandable option")]
        public Size CustomSize
        {
            get
            {
                return optionCustomSize;
            }
            set
            {
                optionCustomSize = value;
            }
        }
        #endregion Properties

        #region Event Handlers
        /// <summary>
        /// Handles "Activate" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when Visual Studio wants to activate this page.  
        /// </devdoc>
        /// <remarks>If the Cancel property of the event is set to true, the page is not activated.</remarks>
        protected override void OnActivate(CancelEventArgs e)
        {
//            DialogResult result = WinFormsHelper.ShowMessageBox(Resources.MessageOnActivateEntered, Resources.MessageOnActivateEntered, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
//
//            if (result == DialogResult.Cancel)
//            {
//                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Cancelled the OnActivate event"));
//                e.Cancel = true;
//            }

            base.OnActivate(e);
        }

        /// <summary>
        /// Handles "Close" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This event is raised when the page is closed.
        /// </devdoc>
//        protected override void OnClosed(EventArgs e)
//        {
//            WinFormsHelper.ShowMessageBox(Resources.MessageOnClosed);
//        }

        /// <summary>
        /// Handles "Deactive" messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when VS wants to deactivate this
        /// page.  If true is set for the Cancel property of the event, 
        /// the page is not deactivated.
        /// </devdoc>
        /// <remarks>
        /// A "Deactive" message is sent when a dialog page's user interface 
        /// window loses focus or is minimized but is not closed.
        /// </remarks>
//        protected override void OnDeactivate(CancelEventArgs e)
//        {
//            DialogResult result = WinFormsHelper.ShowMessageBox(Resources.MessageOnDeactivateEntered, Resources.MessageOnDeactivateEntered, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
//
//            if (result == DialogResult.Cancel)
//            {
//                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Cancelled the OnDeactivate event"));
//                e.Cancel = true;
//            }
//        }

        /// <summary>
        /// Handles Apply messages from the Visual Studio environment.
        /// </summary>
        /// <devdoc>
        /// This method is called when VS wants to save the user's 
        /// changes then the dialog is dismissed.
        /// </devdoc>
//        protected override void OnApply(PageApplyEventArgs e)
//        {
//            DialogResult result = WinFormsHelper.ShowMessageBox(Resources.MessageOnApplyEntered);
//
//            if (result == DialogResult.Cancel)
//            {
//                Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Cancelled the OnApply event"));
//                e.ApplyBehavior = ApplyKind.Cancel;
//            }
//            else
//            {
//                base.OnApply(e);
//            }
//
//            WinFormsHelper.ShowMessageBox(Resources.MessageOnApply);
//        }

        #endregion Event Handlers
    }
}
