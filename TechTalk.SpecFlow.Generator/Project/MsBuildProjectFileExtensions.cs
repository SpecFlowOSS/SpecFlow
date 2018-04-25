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
            return project.Items.Where(x => x.IsNonCompilingItem())
                                            .Where(x => x.IsFeatureFile() || x.IsExcelFeatureFile());
        }

        private static bool IsNonCompilingItem(this ProjectItem x)
        {
            return (x.ItemType == "Content" || x.ItemType == "None");
        }

        private static bool IsExcelFeatureFile(this ProjectItem x)
        {
            return x.EvaluatedInclude.EndsWith(".feature.xlsx", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsFeatureFile(this ProjectItem x)
        {
            return x.EvaluatedInclude.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase);
        }

        public static ProjectItem ApplicationConfigurationFile(this Microsoft.Build.Evaluation.Project project)
        {
            return project.Items.FirstOrDefault(x => IsNonCompilingItem(x) &&
                                                                 Path.GetFileName(x.EvaluatedInclude).Equals("app.config", StringComparison.InvariantCultureIgnoreCase));

        }
    }
}