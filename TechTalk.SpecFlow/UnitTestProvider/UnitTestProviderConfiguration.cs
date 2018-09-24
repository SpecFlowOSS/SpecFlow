using System;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class UnitTestProviderConfiguration
    {
        public void UseUnitTestProvider(string unitTestProviderName)
        {
            if (unitTestProviderName.IsNullOrEmpty())
                throw new ArgumentNullException();

            if (UnitTestProvider.IsNotNullOrEmpty() && UnitTestProvider != unitTestProviderName)
                throw new Exception("Unit test Provider already set.");

            UnitTestProvider = unitTestProviderName;
        }

        public string UnitTestProvider { get; private set; }
    }
}
