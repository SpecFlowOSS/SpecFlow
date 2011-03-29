
namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class MsTestWP7RuntimeProvider : MsTestRuntimeProvider 
    {
        private const string MSTEST_ASSEMBLY = "Microsoft.VisualStudio.QualityTools.UnitTesting.Silverlight";

        public override string AssemblyName
        {
            get
            {
                return MSTEST_ASSEMBLY;
            }
        }
    }
}
