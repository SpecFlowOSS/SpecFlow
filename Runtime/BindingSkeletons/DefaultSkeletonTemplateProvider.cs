using System;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    using System.Reflection;

    public class ResourceSkeletonTemplateProvider : FileBasedSkeletonTemplateProvider
    {
        protected override string GetTemplateFileContent()
        {
#if WINRT
            var resourceStream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("TechTalk.Specflow.WindowsStore.SharedRuntime.DefaultSkeletonTemplates.sftemplate");
#else
            var resourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), "DefaultSkeletonTemplates.sftemplate");
#endif
            if (resourceStream == null)
                throw new SpecFlowException("Missing resource: DefaultSkeletonTemplates.sftemplate");

            using (var reader = new StreamReader(resourceStream))
                return reader.ReadToEnd();
        }
    }

    public class DefaultSkeletonTemplateProvider : FileBasedSkeletonTemplateProvider
    {
        private readonly ResourceSkeletonTemplateProvider resourceSkeletonTemplateProvider;

        public DefaultSkeletonTemplateProvider(ResourceSkeletonTemplateProvider resourceSkeletonTemplateProvider)
        {
            this.resourceSkeletonTemplateProvider = resourceSkeletonTemplateProvider;
        }

        protected override string GetTemplateFileContent()
        {
#if SILVERLIGHT || WINRT
            return "";
#else
            string templateFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"SpecFlow\SkeletonTemplates.sftemplate");
            if (!File.Exists(templateFilePath))
                return "";

            return File.ReadAllText(templateFilePath);
#endif
        }

        protected internal override string GetTemplate(string key)
        {
            return base.GetTemplate(key) ?? resourceSkeletonTemplateProvider.GetTemplate(key);
        }
    }
}