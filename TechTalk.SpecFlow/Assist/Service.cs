using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist.ValueComparers;
using TechTalk.SpecFlow.Assist.ValueRetrievers;

namespace TechTalk.SpecFlow.Assist
{
    public class Service
    {

        private List<IValueComparer> _registeredValueComparers;
        private ServiceComponentList<IValueRetriever> _registeredValueRetrievers;

        public IEnumerable<IValueComparer> ValueComparers => _registeredValueComparers;
        public IEnumerable<IValueRetriever> ValueRetrievers => _registeredValueRetrievers;

        public static Service Instance { get; internal set; }

        static Service()
        {
            Instance = new Service();
        }

        public Service()
        {
            RestoreDefaults();
        }

        public void RestoreDefaults()
        {
            _registeredValueComparers = new List<IValueComparer>();
            _registeredValueRetrievers = new SpecFlowDefaultValueRetrieverList();
            RegisterSpecFlowDefaults();
        }

        public void RegisterValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Insert(0, valueComparer);
        }

        public void RegisterDefaultValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Add(valueComparer);
        }

        public void UnregisterValueComparer(IValueComparer valueComparer)
        {
            _registeredValueComparers.Remove(valueComparer);
        }

        public void RegisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Register(valueRetriever);
        }

        public void UnregisterValueRetriever(IValueRetriever valueRetriever)
        {
            _registeredValueRetrievers.Unregister(valueRetriever);
        }

        public void RegisterSpecFlowDefaults()
        {
            RegisterValueComparer(new DateTimeValueComparer());
            RegisterValueComparer(new BoolValueComparer());
            RegisterValueComparer(new GuidValueComparer(new GuidValueRetriever()));
            RegisterValueComparer(new DecimalValueComparer());
            RegisterValueComparer(new DoubleValueComparer());
            RegisterValueComparer(new FloatValueComparer());
            RegisterDefaultValueComparer(new DefaultValueComparer());

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

