using System;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public abstract class DataSourceSelector
    {
        public virtual bool TryEvaluate(DataValue dataValue, out DataValue result)
        {
            result = EvaluateInternal(dataValue, false);
            return result != null;
        }

        public virtual DataValue Evaluate(DataValue dataValue)
        {
            return EvaluateInternal(dataValue, true);
        }
        
        public abstract DataValue EvaluateInternal(DataValue dataValue, bool throwException);
    }
}
