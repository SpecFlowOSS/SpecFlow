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
        public const string guidSpecFlowCmdSetString = "fe3e8cdf-2b5c-45cf-8493-4f0a43cfe16f";
        public const string guidDynamicMenuDevelopmentGenerateOption = "9d9046da-94f8-4fd0-8a00-92bf4f6defa8";

        public static readonly Guid guidSpecFlowCmdSet = new Guid(guidSpecFlowCmdSetString);
        public static readonly Guid guidSpecFlowGenerateOption = new Guid(guidDynamicMenuDevelopmentGenerateOption);
    };

    static class PkgCmdIDList
    {
        public const uint cmdidGenerate = 0x1009;
        public const uint cmdidGenerateGroup = 0x1020;
    };
}
