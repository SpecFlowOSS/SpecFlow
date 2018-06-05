using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class SpecFlowProject
    {
        public ProjectSettings ProjectSettings { get; set; }
        public SpecFlowProjectConfiguration Configuration { get; set; }

        public SpecFlowProject()
        {
            ProjectSettings = new ProjectSettings();
            Configuration = new SpecFlowProjectConfiguration();
        }
    }
}