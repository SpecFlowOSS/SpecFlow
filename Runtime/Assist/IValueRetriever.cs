using System;

namespace TechTalk.SpecFlow
{
    public interface IValueRetriever
    {
        object ExtractValueFromRow(TableRow row, Type targetType);
    }
}