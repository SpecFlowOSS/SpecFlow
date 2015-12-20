using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist.ValueRetrievers;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Assist
{
    internal class TEHelpers
    {
        private Service service;

        public TEHelpers(Service service)
        {
            this.service = service;
        }

        internal T CreateTheInstanceWithTheDefaultConstructor<T>(Table table)
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            LoadInstanceWithKeyValuePairs(table, instance);
            return instance;
        }

        internal T CreateTheInstanceWithTheValuesFromTheTable<T>(Table table)
        {
            var constructor = GetConstructorMatchingToColumnNames<T>(table);
            if (constructor == null)
                throw new MissingMethodException(string.Format("Unable to find a suitable constructor to create instance of {0}", typeof(T).Name));

            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, typeof(T));

            var constructorParameters = constructor.GetParameters();
            var parameterValues = new object[constructorParameters.Length];
            for (var parameterIndex = 0; parameterIndex < constructorParameters.Length; parameterIndex++)
            {
                var parameterName = constructorParameters[parameterIndex].Name;
                var member = (from m in membersThatNeedToBeSet
                                where m.MemberName == parameterName
                                select m).FirstOrDefault();
                if (member != null)
                    parameterValues[parameterIndex] = member.GetValue();
            }
            return (T)constructor.Invoke(parameterValues);
        }

        internal bool ThisTypeHasADefaultConstructor<T>()
        {
            return typeof(T).GetConstructors()
                       .Where(c => c.GetParameters().Length == 0)
                       .Count() > 0;
        }

        internal ConstructorInfo GetConstructorMatchingToColumnNames<T>(Table table)
        {
            var projectedPropertyNames = from property in typeof(T).GetProperties()
                                         from row in table.Rows
                                         where IsMemberMatchingToColumnName(property, row.Id())
                                         select property.Name;

            return (from constructor in typeof(T).GetConstructors()
                    where projectedPropertyNames.Except(
                        from parameter in constructor.GetParameters()
                        select parameter.Name).Count() == 0
                    select constructor).FirstOrDefault();
        }

        internal bool IsMemberMatchingToColumnName(MemberInfo member, string columnName)
        {
            return MatchesThisColumnName(member.Name, columnName);
        }

        internal bool MatchesThisColumnName(string propertyName, string columnName)
        {
            var normalizedColumnName = NormalizePropertyNameToMatchAgainstAColumnName(RemoveAllCharactersThatAreNotValidInAPropertyName(columnName));
            var normalizedPropertyName = NormalizePropertyNameToMatchAgainstAColumnName(propertyName);

            return normalizedPropertyName.Equals(normalizedColumnName, StringComparison.OrdinalIgnoreCase);
        }

        internal string RemoveAllCharactersThatAreNotValidInAPropertyName(string name)
        {
            //Unicode groups allowed: Lu, Ll, Lt, Lm, Lo, Nl or Nd see https://msdn.microsoft.com/en-us/library/aa664670%28v=vs.71%29.aspx
            return new Regex(@"[^\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}_]").Replace(name, string.Empty);
        }

        internal string NormalizePropertyNameToMatchAgainstAColumnName(string name)
        {
            // we remove underscores, because they should be equivalent to spaces that were removed too from the column names
            return name.Replace("_", string.Empty);
        }

        internal void LoadInstanceWithKeyValuePairs(Table table, object instance)
        {
            var membersThatNeedToBeSet = GetMembersThatNeedToBeSet(table, instance.GetType());

            membersThatNeedToBeSet.ToList()
                .ForEach(x => x.Setter(instance, x.GetValue()));
        }

        internal IEnumerable<MemberHandler> GetMembersThatNeedToBeSet(Table table, Type type)
        {
            var properties = from property in type.GetProperties()
                             from row in table.Rows
                             where TheseTypesMatch(type, property.PropertyType, row)
                                   && IsMemberMatchingToColumnName(property, row.Id())
                select new MemberHandler(service) { Type = type, Row = row, MemberName = property.Name, PropertyType = property.PropertyType, Setter = (i, v) => property.SetValue(i, v, null) };

            var fields = from field in type.GetFields()
                             from row in table.Rows
                             where TheseTypesMatch(type, field.FieldType, row)
                                   && IsMemberMatchingToColumnName(field, row.Id())
                select new MemberHandler(service) { Type = type, Row = row, MemberName = field.Name, PropertyType = field.FieldType, Setter = (i, v) => field.SetValue(i, v) };

            var memberHandlers = new List<MemberHandler>();

            memberHandlers.AddRange(properties);
            memberHandlers.AddRange(fields);

            return memberHandlers;
        }

        private bool TheseTypesMatch(Type targetType, Type memberType, TableRow row)
        {
            return service.GetValueRetrieverFor(row, targetType, memberType) != null;
        }

        internal class MemberHandler
        {
            private Service service;
            public TableRow Row { get; set; }
            public string MemberName { get; set; }
            public Action<object, object> Setter { get; set; }
            public Type Type { get; set; }
            public Type PropertyType { get; set; }

            public MemberHandler(Service service)
            {
                this.service = service;
            }

            public object GetValue()
            {
                var valueRetriever = service.GetValueRetrieverFor(Row, Type, PropertyType);
                return valueRetriever.Retrieve(new KeyValuePair<string, string>(Row[0], Row[1]), Type, PropertyType);
            }
        }

        internal Table GetTheProperInstanceTable(Table table, Type type)
        {
            return ThisIsAVerticalTable(table, type)
                       ? table
                       : FlipThisHorizontalTableToAVerticalTable(table);
        }

        private Table FlipThisHorizontalTableToAVerticalTable(Table table)
        {
            return new PivotTable(table).GetInstanceTable(0);
        }

        private bool ThisIsAVerticalTable(Table table, Type type)
        {
            if (TheHeaderIsTheOldFieldValuePair(table))
                return true;
            return (table.Rows.Count() != 1) || (table.Header.Count() == 2 && TheFirstRowValueIsTheNameOfAProperty(table, type));
        }

        private bool TheHeaderIsTheOldFieldValuePair(Table table)
        {
            return table.Header.Count() == 2 && table.Header.First() == "Field" && table.Header.Last() == "Value";
        }

        private bool TheFirstRowValueIsTheNameOfAProperty(Table table, Type type)
        {
            var firstRowValue = table.Rows[0][table.Header.First()];
            return type.GetProperties()
                .Any(property => IsMemberMatchingToColumnName(property, firstRowValue));
        }
    }
}
