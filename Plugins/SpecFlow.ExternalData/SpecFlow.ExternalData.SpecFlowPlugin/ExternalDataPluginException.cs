using System;

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public class ExternalDataPluginException : Exception
    {
        public ExternalDataPluginException(string message) : base(message)
        {
        }

        public ExternalDataPluginException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
