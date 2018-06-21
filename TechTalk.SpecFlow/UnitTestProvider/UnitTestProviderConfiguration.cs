using System;

namespace TechTalk.SpecFlow.UnitTestProvider
{
    public class UnitTestProviderConfiguration
    {
        public void UseUnitTestProvider(string unitTestProviderName)
        {
            //throws an exception if UnitTestProvider is already set
            if(!UnitTestProvider.IsNullOrEmpty())
                throw new Exception("Unit test Provider already set.");

            UnitTestProvider = unitTestProviderName;
        }

        public string UnitTestProvider { get; private set; }
    }
}
