using System;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    internal static class ReSharperCommandGroups
    {
        public static readonly Guid CommandGroup = new Guid("47f03277-5055-4922-899c-0f7f30d26bf1");
    }

    internal enum ReSharperCommand
    {
        GotoDeclaration = 48,
        LineComment = 37,
        UnitTestRunContext = 2263,
        UnitTestDebugContext = 2264
    }
}