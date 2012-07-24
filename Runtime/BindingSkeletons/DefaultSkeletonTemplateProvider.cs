using System;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.BindingSkeletons
{
    public class DefaultSkeletonTemplateProvider : FileBasedSkeletonTemplateProvider
    {
        protected override string GetTemplateFileContent()
        {
            var resourceStream = GetType().Assembly.GetManifestResourceStream(GetType(), "DefaultSkeletonTemplates.sftemplate");
            if (resourceStream == null)
                throw new SpecFlowException("Missing resource: DefaultSkeletonTemplates.sftemplate");

            using (var reader = new StreamReader(resourceStream))
                return reader.ReadToEnd();
        }

        protected override string MissingTemplate(string key)
        {
            return "undefined template";
        }
    }
}