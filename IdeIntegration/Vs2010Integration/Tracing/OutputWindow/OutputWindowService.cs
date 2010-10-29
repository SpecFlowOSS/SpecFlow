using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow
{
    [Export(typeof(IOutputWindowService))]
    internal sealed class OutputWindowService : IOutputWindowService
    {
        [Import]
        public SVsServiceProvider GlobalServiceProvider = null;

        [ImportMany]
        internal List<Lazy<OutputWindowDefinition, IOutputWindowDefinitionMetadata>> OutputWindowDefinitions = null;

        private readonly Dictionary<string, Guid> outputWindows =
            new Dictionary<string, Guid>
                {
                { PredefinedOutputWindowPanes.Build, VSConstants.GUID_BuildOutputWindowPane },
                { PredefinedOutputWindowPanes.Debug, VSConstants.GUID_OutWindowDebugPane },
                { PredefinedOutputWindowPanes.General, VSConstants.GUID_OutWindowGeneralPane },
            };

        private readonly Dictionary<string, IOutputWindowPane> panes = new Dictionary<string, IOutputWindowPane>();

        public IOutputWindowPane TryGetPane(string name)
        {
            IOutputWindowPane pane;
            if (panes.TryGetValue(name, out pane))
                return pane;

            var olesp = (IOleServiceProvider)GlobalServiceProvider.GetService(typeof(IOleServiceProvider));
            var outputWindow = olesp.TryGetGlobalService<SVsOutputWindow, IVsOutputWindow>();
            if (outputWindow == null)
                return null;

            Guid guid;
            if (!outputWindows.TryGetValue(name, out guid))
            {
                var definition = OutputWindowDefinitions.FirstOrDefault(lazy => lazy.Metadata.Name.Equals(name));
                if (definition == null)
                    return null;

                guid = Guid.NewGuid();
                // this controls whether the pane is listed in the output panes dropdown list, *not* whether the pane is initially selected
                const bool visible = true;
                const bool clearWithSolution = false;

                if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.CreatePane(ref guid, definition.Metadata.Name, Convert.ToInt32(visible), Convert.ToInt32(clearWithSolution)))))
                    return null;

                outputWindows.Add(definition.Metadata.Name, guid);
            }

            IVsOutputWindowPane vspane = null;
            if (ErrorHandler.Failed(ErrorHandler.CallWithCOMConvention(() => outputWindow.GetPane(ref guid, out vspane))))
                return null;

            pane = new VsOutputWindowPaneAdapter(vspane);
            panes[name] = pane;
            return pane;
        }
    }
}
