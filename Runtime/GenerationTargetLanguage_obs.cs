using System;

namespace TechTalk.SpecFlow
{
    [Obsolete("Use ProgrammingLanguage enum")]
    public enum GenerationTargetLanguage
    {
        CSharp = ProgrammingLanguage.CSharp,
        VB = ProgrammingLanguage.VB,
        Other = ProgrammingLanguage.Other
    }
}