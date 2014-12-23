using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace TechTalk.SpecFlow.Vs2010Integration.UI
{
    internal static class VsContextMenuManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
            public POINT()
            {
                x = 0;
                y = 0;
            }
           
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public static void ShowContextMenu(ContextMenuStrip contextMenu, DTE dte)
        {
            try
            {
                var serviceProvider = new ServiceProvider(dte as IServiceProvider);

                IVsUIShellOpenDocument sod = (IVsUIShellOpenDocument)serviceProvider.GetService(typeof(SVsUIShellOpenDocument));
                IVsUIHierarchy targetHier;
                uint[] targetId = new uint[1];
                IVsWindowFrame targetFrame;
                int isOpen;
                Guid viewId = new Guid(LogicalViewID.Primary);
                sod.IsDocumentOpen(null, 0, dte.ActiveWindow.Document.FullName,
                                   ref viewId, 0, out targetHier, targetId,
                                   out targetFrame, out isOpen);

                IVsTextView textView = VsShellUtilities.GetTextView(targetFrame);
                TextSelection selection = (TextSelection)dte.ActiveWindow.Document.Selection;
                Microsoft.VisualStudio.OLE.Interop.POINT[] interopPoint = new Microsoft.VisualStudio.OLE.Interop.POINT[1];
                textView.GetPointOfLineColumn(selection.ActivePoint.Line, selection.ActivePoint.LineCharOffset, interopPoint);

                POINT p = new POINT(interopPoint[0].x, interopPoint[0].y);

                ClientToScreen(textView.GetWindowHandle(), p);

                contextMenu.Show(new Point(p.x, p.y));
            }
            catch (Exception)
            {
                contextMenu.Show();
            }
        }
    }
}