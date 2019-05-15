using System;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages.Configuration;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class BoDiSinkFactory : ISinkFactory
    {
        private readonly Type _cucumberMessageSinkType = typeof(ICucumberMessageSink);
        private readonly IObjectContainer _objectContainer;

        public BoDiSinkFactory(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        public Result<ICucumberMessageSink> FromConfiguration(ISinkConfiguration sinkConfiguration)
        {
            try
            {
                var sinkType = Type.GetType(sinkConfiguration.TypeName);
                if (!IsCompatibleSinkType(sinkType))
                {
                    return Result<ICucumberMessageSink>.Failure();
                }

                var resolvedSink = _objectContainer.Resolve(sinkType);

                if (resolvedSink is ICucumberMessageSink cucumberMessageSink)
                {
                    return Result<ICucumberMessageSink>.Success(cucumberMessageSink);
                }

                return Result<ICucumberMessageSink>.Failure();
            }
            catch (ObjectContainerException)
            {
                return Result<ICucumberMessageSink>.Failure();
            }
        }

        public bool IsCompatibleSinkType(Type sinkTypeNullable)
        {
            return sinkTypeNullable is Type sinkType
                   && sinkType.IsClass
                   && sinkType.GetInterfaces().Contains(_cucumberMessageSinkType);
        }
    }
}
