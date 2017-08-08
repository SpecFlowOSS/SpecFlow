using System;
using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    /// <summary>
    /// Resolves user created test objects from different scopes (scenario, feature, test thread).
    /// </summary>
    /// <remarks>
    /// <para>Notes to the implementors:</para>
    /// <para>
    /// The test objects might be dependent on particular SpecFlow infrastructure, therefore the implemented 
    /// resolution logic should support resolving the following objects (from the provided SpecFlow container):
    /// <see cref="ScenarioContext"/>, <see cref="FeatureContext"/>, <see cref="TestThreadContext"/> and 
    /// <see cref="IObjectContainer"/> (to be able to resolve any other SpecFlow infrastucture). So basically 
    /// the resolution of these classes has to be forwarded to the original container.
    /// </para>
    /// <para>
    /// If the resolved (top level) object implements <see cref="IContainerDependentObject"/>, the method 
    /// <see cref="IContainerDependentObject.SetObjectContainer"/> must be called, passing in the original 
    /// SpecFlow container. (The <see cref="Steps"/> base class needs this.)
    /// </para>
    /// </remarks>
    public interface ITestObjectResolver
    {
        object ResolveBindingInstance(Type bindingType, IObjectContainer container);
    }
}