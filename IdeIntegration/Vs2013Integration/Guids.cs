using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Vs2010Integration
{
    static class GuidList
    {
        public const string vsContextGuidSilverlightProject = "{CB22EE0E-4072-4ae7-96E2-90FCCF879544}";

        public const string guidSpecFlowPkgString = "5d978b7f-8f91-41c1-b7ba-0b4c056118e8";
        public const string guidSpecFlowCmdSetString = "8c202d78-762d-4079-ac0e-282ee24b44b0";

        public static readonly Guid guidSpecFlowCmdSet = new Guid(guidSpecFlowCmdSetString);
    };

    internal static class PkgCmdIDList
    {
        public const uint FileContextMenuGroup = 0x1020;
        public const uint ProjectContextMenuGroup = 0x1021;
        public const uint CodeWindowContextMenuGroup = 0x1022;
        public const uint FolderContextMenuGroup = 0x1023;

        public const uint cmdidGenerateStepDefinitionSkeleton = 0x0100;
        public const uint cmdidRunScenarios = 0x0101;
        public const uint cmdidDebugScenarios = 0x0102;
        public const uint cmdidGoToStepDefinition = 0x0103;
        public const uint cmdidReGenerateAll = 0x0104;
        public const uint cmdidGoToSteps = 0x0105;
        public const uint cmdidContextDependentNavigation = 0x0106;
    };

    public enum SpecFlowCmdSet : uint 
    {
        GenerateStepDefinitionSkeleton = PkgCmdIDList.cmdidGenerateStepDefinitionSkeleton,
        RunScenarios = PkgCmdIDList.cmdidRunScenarios,
        DebugScenarios = PkgCmdIDList.cmdidDebugScenarios,
        GoToStepDefinition = PkgCmdIDList.cmdidGoToStepDefinition,
        ReGenerateAll = PkgCmdIDList.cmdidReGenerateAll,
        GoToSteps = PkgCmdIDList.cmdidGoToSteps,
        ContextDependentNavigation = PkgCmdIDList.cmdidContextDependentNavigation,
    }
}
