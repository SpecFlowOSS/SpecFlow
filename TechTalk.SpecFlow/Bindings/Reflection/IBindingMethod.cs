﻿using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings.Reflection
{
    public interface IBindingMethod
    {
        /// <summary>
        /// The declaring type of the method.
        /// </summary>
        IBindingType Type { get; }

        string Name { get; }

        /// <summary>
        /// Gets the obsoletion.
        /// </summary>
        BindingObsoletion Obsoletion { get; }

        IEnumerable<IBindingParameter> Parameters { get; }

        /// <summary>
        /// The return type of the method or <value>null</value> for void methods.
        /// </summary>
        IBindingType ReturnType { get; }
    }
}