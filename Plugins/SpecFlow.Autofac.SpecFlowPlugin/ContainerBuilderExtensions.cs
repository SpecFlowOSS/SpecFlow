using System;
using System.Linq;
using Autofac;
using TechTalk.SpecFlow;

namespace SpecFlow.Autofac.SpecFlowPlugin
{
    /// <summary>
    /// Container builder extension methods.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Add SpecFlow binding for classes in the assembly where typeof TAssemblyType resides.
        /// </summary>
        /// <typeparam name="TAssemblyType">The type in an assembly to search for bindings.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>The builder.</returns>
        public static ContainerBuilder AddSpecFlowBindings<TAssemblyType>(this ContainerBuilder builder) => builder.AddSpecFlowBindings(typeof(TAssemblyType));

        /// <summary>
        /// Add SpecFlow binding for classes in the assembly where the type resides.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="type">The type in an assembly to search for bindings.</param>
        /// <returns>The builder.</returns>
        public static ContainerBuilder AddSpecFlowBindings(this ContainerBuilder builder, Type type)
        {
            builder
               .RegisterAssemblyTypes(type.Assembly)
               .Where(t => Attribute.IsDefined(t, typeof(BindingAttribute)))
               .SingleInstance();
            return builder;
        }
    }
}
