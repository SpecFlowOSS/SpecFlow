using System;

namespace TechTalk.SpecFlow.Async
{
    internal class UnitTestingFrameworkHelper
    {
        private static ReflectionHelper reflectionHelper;

        private readonly object testClassInstance;

        public UnitTestingFrameworkHelper(object testClassInstance)
        {
            var workItemTestType = AssertTestClassDerivesFromWorkItemTest(testClassInstance);
            this.testClassInstance = testClassInstance;
            InitialiseReflectionHelper(workItemTestType);
        }

        private static Type AssertTestClassDerivesFromWorkItemTest(object testClassInstance)
        {
            var type = testClassInstance.GetType();
            while(type != null && type.FullName != "Microsoft.Silverlight.Testing.WorkItemTest")
                type = type.BaseType;

            if (type == null)
                throw new InvalidOperationException("testClassInstance must be an instance of WorkItemTest");

            return type;
        }

        private static void InitialiseReflectionHelper(Type workItemTestType)
        {
            if (reflectionHelper == null)
                reflectionHelper = new ReflectionHelper(workItemTestType);
        }

        public object CreateCompositeWorkItem(Action<Exception> onException)
        {
            return reflectionHelper.CreateCompositeWorkItem(onException);
        }

        public void PushDefaultWorkItem(object compositeWorkItem)
        {
            reflectionHelper.PushDefaultWorkItem(testClassInstance, compositeWorkItem);
        }

        public void EnqueueWorkItem(object compositeWorkItem)
        {
            reflectionHelper.EnqueueWorkItem(testClassInstance, compositeWorkItem);
        }

        public void PopDefaultWorkItem()
        {
            reflectionHelper.PopDefaultWorkItem(testClassInstance);
        }
    }
}