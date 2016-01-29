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

            return project.AllEvaluatedItems.Where(x => IsNonCompilingItem(x) &&
                                                        x.EvaluatedInclude.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsNonCompilingItem(ProjectItem x)
        {
            return (x.ItemType == "Content" || x.ItemType == "None");
        }

        public static ProjectItem ApplicationConfigurationFile(this Microsoft.Build.Evaluation.Project project)
        {
            return project.AllEvaluatedItems.FirstOrDefault(x => IsNonCompilingItem(x) &&
                                                                 Path.GetFileName(x.EvaluatedInclude).Equals("app.config", StringComparison.InvariantCultureIgnoreCase));

        }
    }
}