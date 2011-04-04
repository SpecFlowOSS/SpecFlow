using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Tools.MsBuild
{
    public class GenerateAll : GeneratorTaskBase
    {
        public bool VerboseOutput { get; set; }
        public bool ForceGeneration { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        protected override void DoExecute()
        {
            SpecFlowProject specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(ProjectPath);

            ITraceListener traceListener = VerboseOutput ? (ITraceListener)new TextWriterTraceListener(GetMessageWriter(MessageImportance.High), "SpecFlow: ") : new NullListener();
            BatchGenerator batchGenerator = new MsBuildBatchGenerator(traceListener, new TestGeneratorFactory(), this);
            batchGenerator.ProcessProject(specFlowProject, ForceGeneration);
        }
    }
}