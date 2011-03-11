using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TechTalk.SpecFlow.Vs2010Integration.Utils
{
    public class RegexDictionary<T> : IEnumerable<T>
    {
        private readonly Func<T, Regex> regexLocator;
        private readonly int hashPrefixLenght;
        private readonly Dictionary<string, List<T>> hashedList;

        public RegexDictionary(Func<T, Regex> regexLocator, int hashPrefixLenght = 3)
        {
            this.regexLocator = regexLocator;
            this.hashPrefixLenght = hashPrefixLenght;
            hashedList = new Dictionary<string, List<T>>(StringComparer.CurrentCultureIgnoreCase);
        }

        public void Add(T regexItem)
        {
            var regex = regexLocator(regexItem);
            var hashKey = GetKey(regex);

            List<T> list;
            if (!hashedList.TryGetValue(hashKey, out list))
            {
                list = new List<T>();
                hashedList.Add(hashKey, list);
            }

            list.Add(regexItem);
        }

        public IEnumerable<T> GetMatchingItems(string value)
        {
            var key = GetKey(value);

            IEnumerable<T> potentialItems = GetPotentialItems(key);

            return from regexItem in potentialItems 
                   let regex = regexLocator(regexItem) 
                   where regex == null || regex.Match(value).Success 
                   select regexItem;
        }

        private IEnumerable<T> GetPotentialItems(string key)
        {
            IEnumerable<T> potentialItems = Enumerable.Empty<T>();
            List<T> list;
            if (hashedList.TryGetValue(key, out list))
                potentialItems = potentialItems.Concat(list);
            if (hashedList.TryGetValue(string.Empty, out list))
                potentialItems = potentialItems.Concat(list);
            return potentialItems;
        }

        public IEnumerable<T> GetRelatedItems(Regex regex)
        {
            var hashKey = GetKey(regex);
            return GetPotentialItems(hashKey);
        }

        private string GetKey(Regex regex)
        {
            if (regex == null)
                return string.Empty;

            var key = GetKey(regex.ToString());
            if (key.Length == 0 || !key.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                return string.Empty;
            return key;
        }

        private string GetKey(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Length > hashPrefixLenght ? value.Substring(0, hashPrefixLenght) : value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return hashedList.Values.SelectMany(items => items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
