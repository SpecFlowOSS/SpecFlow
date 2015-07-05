using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    /// <summary>
    /// A class that will retrieve an object's actual value from a key->value in a table.
    /// </summary>
    public interface IValueRetriever
    {
        /// <summary>
        /// Determines if this retriever can retrieve the actual value from a key->value set in a table.
        /// </summary>
        /// <returns><c>true</c> if this instance can retrieve the specified key->value; otherwise, <c>false</c>.</returns>
        /// <param name="keyValuePair">Key value pair.</param>
        /// <param name="type">The type of the object that is being built from the table.</param>
        bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type type);

        /// <summary>
        /// Retrieve the value from a key-> value set, as the expected type on targetType.
        /// </summary>
        /// <param name="keyValuePair">Key value pair.</param>
        /// <param name="targetType">The type of the ojbect that is being built from the table.</param>
        object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType);
    }
}