using System;
using System.Configuration;

namespace TechTalk.SpecFlow.Configuration
{
    public static class ConfigurationServices
    {
        public static TInterface CreateInstance<TInterface>(Type type)
        {
            // do not use ErrorProvider for thowing exceptions here, because of the potential
            // infinite loop
            try
            {
                return (TInterface)Activator.CreateInstance(type);
            }
            catch (InvalidCastException)
            {
                throw new ConfigurationErrorsException(
                    String.Format("The specified type '{0}' does not implement interface '{1}'",
                                  type.FullName, typeof(TInterface).FullName));
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    String.Format("Unable to create instance of type '{0}': {1}",
                                  type.FullName, ex.Message), ex);
            }
        }
        public static TInterface CreateInstance<TInterface>(Type type, params object[] arguments)
        {
            // do not use ErrorProvider for thowing exceptions here, because of the potential
            // infinite loop
            try
            {
                return (TInterface)Activator.CreateInstance(type, arguments);
            }
            catch (InvalidCastException)
            {
                throw new ConfigurationErrorsException(
                    String.Format("The specified type '{0}' does not implement interface '{1}'",
                                  type.FullName, typeof(TInterface).FullName));
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(
                    String.Format("Unable to create instance of type '{0}': {1}",
                                  type.FullName, ex.Message), ex);
            }
        }
    }
}