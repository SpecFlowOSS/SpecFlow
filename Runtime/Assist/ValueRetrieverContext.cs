using System;

namespace TechTalk.SpecFlow.Assist
{
    public class ValueRetrieverContext
    {
        public Type InstanceType { get; private set; }
        public Table Table { get; private set; }
        public TableRow Row { get; private set; }
        public string Field { get; private set; }
        public string Value { get; private set; }

        public ValueRetrieverContext(string value)
        {
            Value = value;
        }

        public ValueRetrieverContext(Type instanceType, Table table, TableRow row)
        {
            InstanceType = instanceType;
            Table = table;
            Row = row;
            Field = row[0];
            Value = row[1];
        }
    }
}