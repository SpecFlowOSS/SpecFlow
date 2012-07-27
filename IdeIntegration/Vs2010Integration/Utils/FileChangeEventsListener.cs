using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    public class FileChangeEventsListener : IVsFileChangeEvents, IDisposable
    {
        private readonly IIdeTracer tracer;
        private IVsFileChangeEx fileChangeEx;
        private readonly List<KeyValuePair<string, uint>> listenInfos = new List<KeyValuePair<string, uint>>();

        public event Action<string> FileChanged;

        public FileChangeEventsListener(IIdeTracer tracer)
        {
            this.tracer = tracer;
            InitNullEvents();

            fileChangeEx = Package.GetGlobalService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;
        }

        private void InitNullEvents()
        {
            FileChanged += _ => { };
        }

        public void StartListeningToFile(string file)
        {
            if (fileChangeEx == null || file == null)
                return;

            file = Path.GetFullPath(file);

            if (listenInfos.Any(i => file.Equals(i.Key, StringComparison.InvariantCultureIgnoreCase)))
                return;

            tracer.Trace("Start listening to file: {0}", this, file);

            uint cookie;
            ErrorHandler.ThrowOnFailure(
                fileChangeEx.AdviseFileChange(
                    file,
                    (uint)(_VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Del),
                    this,
                    out cookie));
            listenInfos.Add(new KeyValuePair<string, uint>(file, cookie));
        }

        public void StopListening()
        {
            if (fileChangeEx != null && listenInfos.Any())
            {
                var cookies = listenInfos.Select(i => i.Value).ToArray();
                listenInfos.Clear();
                foreach (var cookie in cookies)
                {
                    fileChangeEx.UnadviseFileChange(cookie);
                }
            }
        }

        public void Dispose()
        {
            StopListening();
            fileChangeEx = null;
        }

        int IVsFileChangeEvents.FilesChanged(uint numberOfFilesChanged, string[] filesChanged, uint[] rggrfChange)
        {
            for (int i = 0; i < numberOfFilesChanged; i++)
            {
                FileChanged(filesChanged[i]);
            }

            return VSConstants.S_OK;
        }

        int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
        {
            return VSConstants.S_OK;
        }

    }
}