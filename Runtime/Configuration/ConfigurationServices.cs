using System;
using System.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Configuration
{
    public static class ConfigurationServices
    {
        public static TInterface CreateInstance<TInterface>(Type type)
        {
            //HACK: to provide backwards compatibility, until DI is fully introduced
            if (type.GetConstructor(new Type[0]) == null)
            {
                var container = TestRunContainerBuilder.CreateContainer();
                return container.Resolve<TInterface>();
            }

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
    }
}