// Guids.cs
// MUST match guids.h
using System;

namespace TechTalk.SpecFlow.VsIntegration
{
    static class GuidList
    {
        public const string guidTechTalk_SpecFlowPkgString = "3a62f4e2-b3d5-4eed-a62d-4acb4936b6c6";
        public const string guidTechTalk_SpecFlowCmdSetString = "d355cecd-5776-4439-a76a-c0d97f8c246a";

        public static readonly Guid guidTechTalk_SpecFlowCmdSet = new Guid(guidTechTalk_SpecFlowCmdSetString);
    };
}