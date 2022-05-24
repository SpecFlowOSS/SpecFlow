using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public abstract class FileBasedSkeletonTemplateProvider : ISkeletonTemplateProvider
    {
        private const string templateSeparator = ">>>";
        private Dictionary<string, string> templates;

        private Dictionary<string, string> GetTemplates()
        {
            var templates = new Dictionary<string, string>();

            string templateFileContent = GetTemplateFileContent();
            var templateItems = templateFileContent.Split(new[] {templateSeparator}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var templateItem in templateItems.Select(ti => new StringReader(ti)).Select(reader => new { Key = reader.ReadLine(), Body = reader.ReadToEnd() }))
            {
                if (string.IsNullOrEmpty(templateItem.Key))
                    throw new SpecFlowException($"Invalid sftemplate file! Missing title after '{templateSeparator}'.");

                if (templates.ContainsKey(templateItem.Key))
                    throw new SpecFlowException($"Invalid sftemplate file! Duplicate key: '{templateItem.Key}'.");
                templates.Add(templateItem.Key, templateItem.Body);
            }

            return templates;
        }

        protected abstract string GetTemplateFileContent();
        protected virtual string MissingTemplate(string key)
        {
            return "undefined template";
        }

        protected internal virtual string GetTemplate(string key)
        {
            LazyInitializer.EnsureInitialized(ref templates, GetTemplates);

            if (!templates.TryGetValue(key, out string template))
                return null;
            return template;
        }

        public string GetStepDefinitionTemplate(ProgrammingLanguage language, bool withExpression)
        {
            string key = $"{language}/StepDefinition{(withExpression ? "Expression" : "")}";
            string template = GetTemplate(key);
            if (template == null)
                return MissingTemplate(key);

            return template;
        }

        public string GetStepDefinitionClassTemplate(ProgrammingLanguage language)
        {
            string key = $"{language}/StepDefinitionClass";
            string template = GetTemplate(key);
            if (template == null)
                return MissingTemplate(key);

            return template;
        }
    }
}