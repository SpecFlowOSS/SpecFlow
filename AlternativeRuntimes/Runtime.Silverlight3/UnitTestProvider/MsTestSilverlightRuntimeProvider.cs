
namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class MsTestSilverlightRuntimeProvider : MsTestRuntimeProvider 
    {
        private const string MSTEST_ASSEMBLY = "Microsoft.VisualStudio.QualityTools.UnitTesting.Silverlight, Version=2.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

        public override string AssemblyName
        {
            get
            {
                return MSTEST_ASSEMBLY;
            }
        }
    }
}
