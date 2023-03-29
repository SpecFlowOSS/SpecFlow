using System;
using System.Collections.Generic;

namespace SpecFlow.ExternalData.SpecFlowPlugin.DataSources.Selectors
{
    public class FieldNameComparer : IEqualityComparer<string>
    {
        public static readonly FieldNameComparer Value = new();

        private readonly StringComparer _baseComparer = StringComparer.InvariantCulture;

        public bool Equals(string x, string y)
        {
            return _baseComparer.Equals(Normalize(x), Normalize(y));
        }

        public int GetHashCode(string obj) =>
            _baseComparer.GetHashCode(Normalize(obj));

        private static string Normalize(string obj) => obj?.Replace(' ', '_');
    }
}
