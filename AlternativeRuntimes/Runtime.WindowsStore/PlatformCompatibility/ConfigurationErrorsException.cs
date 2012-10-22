using System;
using System.Net;
using System.Runtime.Serialization;


namespace System.Configuration
{
    public class ConfigurationErrorsException : Exception
    {
        public ConfigurationErrorsException()
        {
        }

        public ConfigurationErrorsException(string message) : base(message)
        {
        }

        public ConfigurationErrorsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
