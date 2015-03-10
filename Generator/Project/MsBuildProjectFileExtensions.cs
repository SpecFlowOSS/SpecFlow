using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;

namespace TechTalk.SpecFlow.Generator.Project
{
    static class MsBuildProjectFileExtensions
    {
        public static IEnumerable<ProjectItem> FeatureFiles(this Microsoft.Build.Evaluation.Project project)
        {
            
            return project.AllEvaluatedItems.Where(x => x.ItemType == "None" &&
                                                        x.EvaluatedInclude.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase));
        }

        public static ProjectItem ApplicationConfigurationFile(this Microsoft.Build.Evaluation.Project project)
        {
            return project.AllEvaluatedItems.FirstOrDefault(x => x.ItemType == "Content" &&
                                                                 Path.GetFileName(x.EvaluatedInclude).Equals("app.config", StringComparison.InvariantCultureIgnoreCase));

        }
    }
}