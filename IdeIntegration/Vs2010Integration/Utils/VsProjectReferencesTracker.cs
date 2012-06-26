using System;
using System.Linq;
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

        public VsProjectReferencesTracker(Project project, DteWithEvents dteWithEvents, IIdeTracer tracer)
        {
            this.project = project.Object as VSProject;
            this.dteWithEvents = dteWithEvents;
            this.tracer = tracer;
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
        }

        private void FileChangedOnDisk(string filePath)
        {
            var reference = project.References.OfType<Reference>().FirstOrDefault(r => filePath.Equals(r.Path, StringComparison.InvariantCultureIgnoreCase));
            if (reference != null)
            {
                tracer.Trace("Reference changed on disk: {0}", this, reference.Name);
                OnReferenceChanged(reference);
            }
        }

        private void ReferencesEventsOnReferenceChanged(Reference reference)
        {
            tracer.Trace("Reference changed: {0}", this, reference.Name);
            OnReferenceChanged(reference);
        }

        private void OnReferenceChanged(Reference reference)
        {
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