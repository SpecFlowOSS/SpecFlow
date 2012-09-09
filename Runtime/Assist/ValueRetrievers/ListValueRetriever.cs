using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.Assist.ValueRetrievers
{
    public class ListValueRetriever
    {
        internal object GetValue(string values, Type listType)
        {
            var elementType = listType.GetGenericArguments().Single();

            object list = Activator.CreateInstance(listType);
            var add = listType.GetMethod("Add");
            foreach (var value in values.Split(new[] { "," }, StringSplitOptions.None).Select(v => v.Trim()).Select(v => Convert.ChangeType(v, elementType)))
                add.Invoke(list, new[] { value });

            return list;
        }
    }
}
