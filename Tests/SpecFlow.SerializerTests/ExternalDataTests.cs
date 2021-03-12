using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using Utf8Json;
using Xunit;
using Xunit.Sdk;

namespace SerializerTests
{
    public class ExternalDataTests
    {
        [Fact]
        public void TestDataDeserializationResultsTheSameObject()
        {
            var content = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "testdata.json"));

            var utf8JsonResult = JsonSerializer.Deserialize<dynamic>(content);
            var tinyJsonResult = content.FromJson<dynamic>();

            var utf8Dict = utf8JsonResult as Dictionary<string, object>;
            var tinyDict = tinyJsonResult as Dictionary<string, object>;

            Assert.NotNull(utf8Dict);
            Assert.NotNull(tinyDict);

            var dictEqual = DictionariesEqual(utf8Dict, tinyDict);
            Assert.True(dictEqual);
        }

        private bool DictionariesEqual(Dictionary<string, object> dict1, Dictionary<string, object> dict2)
        {
            foreach (var pair in dict1)
            {
                if (!dict2.ContainsKey(pair.Key)) throw new XunitException($"missing key: {pair.Key}");

                var dict2Value = dict2[pair.Key];

                if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    DictionariesEqual(pair.Value as Dictionary<string, object>, dict2Value as Dictionary<string, object>);
                }
                else if (pair.Value.GetType() == typeof(List<object>))
                {
                    if (!(pair.Value as List<object>).SequenceEqual(dict2Value as List<object>))
                        throw new XunitException($"values not equal. Value1: {pair.Value} - Value2: {dict2Value}");
                }
                else
                {
                    if (!pair.Value.Equals(dict2Value))
                        throw new XunitException($"values not equal. Value1: {pair.Value} - Value2: {dict2Value}");
                }
            }
            return true;
        }
    }
}
