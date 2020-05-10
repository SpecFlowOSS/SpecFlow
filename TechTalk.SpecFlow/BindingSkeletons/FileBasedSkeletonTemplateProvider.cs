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
        private Dictionary<string, string> templates = null;

        private Dictionary<string, string> GetTemplates()
        {
            var templates = new Dictionary<string, string>();

            string templateFileContent = GetTemplateFileContent();
            var templateItems = templateFileContent.Split(new[] {templateSeparator}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var templateItem in templateItems.Select(ti => new StringReader(ti)).Select(reader => new { Key = reader.ReadLine(), Body = reader.ReadToEnd() }))
            {
                if (string.IsNullOrEmpty(templateItem.Key))
                    throw new SpecFlowException(string.Format("Invalid sftemplate file! Missing title after '{0}'.", templateSeparator));

                if (templates.ContainsKey(templateItem.Key))
                    throw new SpecFlowException(string.Format("Invalid sftemplate file! Duplicate key: '{0}'.", templateItem.Key));
                templates.Add(templateItem.Key, templateItem.Body);
            }

            return templates;
        }

        protected abstract string GetTemplateFileContent();
        protected virtual string MissingTemplate(string key)
        {
            return "undefined template";
        }

        internal protected virtual string GetTemplate(string key)
        {
            LazyInitializer.EnsureInitialized(ref templates, GetTemplates);

            string template;
            if (!templates.TryGetValue(key, out template))
                return null;
            return template;
        }

        public string GetStepDefinitionTemplate(ProgrammingLanguage language, bool withRegex)
        {
            string key = string.Format("{0}/StepDefinition{1}", language, withRegex ? "Regex" : "");
            string template = GetTemplate(key);
            if (template == null)
                return MissingTemplate(key);

            return template;
        }

        public string GetStepDefinitionClassTemplate(ProgrammingLanguage language)
        {
            string key = string.Format("{0}/StepDefinitionClass", language);
            string template = GetTemplate(key);
            if (template == null)
                return MissingTemplate(key);

            return template;
        }
    }
}