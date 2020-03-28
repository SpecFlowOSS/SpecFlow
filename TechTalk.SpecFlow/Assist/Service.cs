using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    public class Service
    {
        public ServiceComponentList<IValueComparer> ValueComparers { get; private set; }
        public ServiceComponentList<IValueRetriever> ValueRetrievers { get; private set; }

        public static Service Instance { get; internal set; } = new Service();

        public Service()
        {
            RestoreDefaults();
        }

        public void RestoreDefaults()
        {
            ValueComparers = new SpecFlowDefaultValueComparerList();
            ValueRetrievers = new SpecFlowDefaultValueRetrieverList();
        }

        [Obsolete("Use ValueComparers.Register")]
        public void RegisterValueComparer(IValueComparer valueComparer)
        {
            ValueComparers.Register(valueComparer);
        }

        [Obsolete("Use ValueComparers.Unregister")]
        public void UnregisterValueComparer(IValueComparer valueComparer)
        {
            ValueComparers.Unregister(valueComparer);
        }

        [Obsolete("Use ValueRetrievers.Register")]
        public void RegisterValueRetriever(IValueRetriever valueRetriever)
        {
            ValueRetrievers.Register(valueRetriever);
        }

        [Obsolete("Use ValueRetrievers.Unregister")]
        public void UnregisterValueRetriever(IValueRetriever valueRetriever)
        {
            ValueRetrievers.Unregister(valueRetriever);
        }

        public IValueRetriever GetValueRetrieverFor(TableRow row, Type targetType, Type propertyType)
        {
            foreach (var valueRetriever in ValueRetrievers)
            {
                if (valueRetriever.CanRetrieve(new KeyValuePair<string, string>(row[0], row[1]), targetType, propertyType))
                    return valueRetriever;
            }
            return null;
        }

    }
}

