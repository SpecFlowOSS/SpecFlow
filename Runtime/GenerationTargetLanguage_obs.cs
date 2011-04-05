using System;

namespace TechTalk.SpecFlow
{
    //obsolete attribute removed to avoid warnings in the generated code (Issue 58)
    //[Obsolete("Use ProgrammingLanguage enum")]
    public enum GenerationTargetLanguage
    {
        CSharp = ProgrammingLanguage.CSharp,
        VB = ProgrammingLanguage.VB,
        Other = ProgrammingLanguage.Other
    }
}