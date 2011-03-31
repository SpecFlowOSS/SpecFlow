using System;

namespace TechTalk.SpecFlow.Generator
{
    public static class GenerationTargetLanguage
    {
        public const string CSharp = "C#";
        public const string VB = "VB";

        public static string GetExtension(string programmingLanguage)
        {
            switch (programmingLanguage)
            {
                case CSharp:
                    return ".cs";
                case VB:
                    return ".vb";
                default:
                    throw new NotSupportedException("Programming language not supported: " + programmingLanguage);
            }
        }
    }
}