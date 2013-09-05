using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using VSLangProj;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    internal class VsProjectReferencesTracker : IDisposable
    {
        private readonly VSProject project;
        private readonly DteWithEvents dteWithEvents;
        private readonly IIdeTracer tracer;

        private ReferencesEvents referencesEvents = null;

        public event Action<Reference> ReferenceAdded;
        public event Action<Reference> ReferenceRemoved;
        public event Action<Reference> ReferenceChanged;

        private readonly Timer timer;
        private readonly Timer triggerReferenceChangedTimer;
        private HashSet<string> filesChangedOnDisk = new HashSet<string>();

        public VsProjectReferencesTracker(Project project, DteWithEvents dteWithEvents, IIdeTracer tracer)
        {
            this.project = project.Object as VSProject;
            this.dteWithEvents = dteWithEvents;
            this.tracer = tracer;
            this.timer = new Timer(HandleFilesChangedOnDisk, null, Timeout.Infinite, Timeout.Infinite);
            this.triggerReferenceChangedTimer = new Timer(TriggerReferenceChange, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartTracking()
        {
            if (project == null)
                return;

            referencesEvents = project.Events.ReferencesEvents;
            referencesEvents.ReferenceAdded += ReferencesEventsOnReferenceAdded;
            referencesEvents.ReferenceRemoved += ReferencesEventsOnReferenceRemoved;
            referencesEvents.ReferenceChanged += ReferencesEventsOnReferenceChanged;

            dteWithEvents.FileChangeEventsListener.FileChanged += FileChangedOnDisk;

            foreach (var reference in project.References.OfType<Reference>())
            {
                StartListeningToReference(reference);
            }
        }

        public void StopTracking()
        {
            if (referencesEvents == null)
                return;

            referencesEvents.ReferenceAdded -= ReferencesEventsOnReferenceAdded;
            referencesEvents.ReferenceRemoved -= ReferencesEventsOnReferenceRemoved;
            referencesEvents.ReferenceChanged -= ReferencesEventsOnReferenceChanged;

            dteWithEvents.FileChangeEventsListener.FileChanged -= FileChangedOnDisk;
            referencesEvents = null;

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            triggerReferenceChangedTimer.Change(Timeout.Infinite, Timeout.Infinite);
            filesChangedOnDisk.Clear();
        }

        private void TriggerReferenceChange(object state)
        {
            if (filesChangedOnDisk.Count == 0)
                return;
            var filesChanged = filesChangedOnDisk.ToArray();

            foreach (var filePath in filesChanged)
            {
                try
                {
                    FindReference(filePath); //this is a dummy call to trigger the change event of VS (to avoid duplicated processing)
                }
                catch(Exception ex)
                {
                    tracer.Trace("Error during reference change triggering: {0}", this, ex);
                }
            }
        }

        private void HandleFilesChangedOnDisk(object _)
        {
            if (filesChangedOnDisk.Count == 0)
                return;
            var filesChanged = filesChangedOnDisk;
            filesChangedOnDisk = new HashSet<string>();

            foreach (var filePath in filesChanged)
            {
                try
                {
                    var reference = FindReference(filePath);
                    if (reference == null)
                        return;

                    OnReferenceChanged(reference);
                }
                catch(Exception ex)
                {
                    tracer.Trace("Error during reference change handling: {0}", this, ex);
                }
            }
        }

        private void FileChangedOnDisk(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension == null || !extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                return;

            tracer.Trace("Reference changed on disk: {0}", this, filePath);
            QueueHandlingFileOnDiskChange(filePath);
        }

        private Reference FindReference(string filePath)
        {
            return project.References.OfType<Reference>().FirstOrDefault(r => filePath.Equals(r.Path, StringComparison.InvariantCultureIgnoreCase));
        }

        private void ReferencesEventsOnReferenceChanged(Reference reference)
        {
            if (string.IsNullOrEmpty(reference.Path))
                return;

            tracer.Trace("Reference changed: {0}", this, reference.Name);
            QueueHandlingFileOnDiskChange(reference.Path);
        }

        private void QueueHandlingFileOnDiskChange(string filePath)
        {
            const int FILE_TRIGGER_DELAY_MSEC = 1500;
            const int FILE_CHANGE_DELAY_MSEC = 2000;

            filesChangedOnDisk.Add(filePath);
            timer.Change(FILE_CHANGE_DELAY_MSEC, Timeout.Infinite);
            triggerReferenceChangedTimer.Change(FILE_TRIGGER_DELAY_MSEC, Timeout.Infinite);
        }

        private void OnReferenceChanged(Reference reference)
        {
            tracer.Trace("Processing reference changed: {0}", this, reference.Name);
            if (ReferenceChanged != null)
                ReferenceChanged(reference);
        }

        private void ReferencesEventsOnReferenceRemoved(Reference reference)
        {
            tracer.Trace("Reference removed: {0}", this, reference.Name);
            if (ReferenceRemoved != null)
                ReferenceRemoved(reference);
        }

        private void ReferencesEventsOnReferenceAdded(Reference reference)
        {
            tracer.Trace("Reference added: {0}", this, reference.Name);

            if (ReferenceAdded != null)
                ReferenceAdded(reference);

            StartListeningToReference(reference);
        }

        private void StartListeningToReference(Reference reference)
        {
            string assemblyPath = reference.Path;
            if (!string.IsNullOrEmpty(assemblyPath) && !assemblyPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), StringComparison.InvariantCultureIgnoreCase))
            {
                dteWithEvents.FileChangeEventsListener.StartListeningToFile(assemblyPath);
            }
        }

        public void Dispose()
        {
            StopTracking();
        }
    }
}