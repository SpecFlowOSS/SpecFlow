using System;

namespace TechTalk.SpecFlow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GenerationTargetLanguageAttribute : Attribute
    {
        public GenerationTargetLanguageAttribute(string language)
        {
            Language = language;
        }

        public string Language { get; private set; }
    }
}
