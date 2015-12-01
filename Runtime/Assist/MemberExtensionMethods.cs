using System.Linq;
using System.Reflection;

namespace TechTalk.SpecFlow.Assist
{
    internal static class MemberExtensionMethods
    {
        public static object GetMemberValue(this object @object, string memberName)
        {
            var property = GetThePropertyOnThisObject(@object, memberName);

            if (property != null)
            {
                return property.GetValue(@object, null);
            }

            var field = GetTheFieldOnThisObject(@object, memberName);
            return field.GetValue(@object);
            
        }        

        public static void SetMemberValue(this object @object, string propertyName, object value)
        {
            var property = GetThePropertyOnThisObject(@object, propertyName);

            if (property != null)
            {
                property.SetValue(@object, value, null);
                return;
            }

            var field = GetTheFieldOnThisObject(@object, propertyName);
            field.SetValue(@object, value);
            
        }


        private static FieldInfo GetTheFieldOnThisObject(object @object, string fieldName)
        {
            var type = @object.GetType();
            return type.GetFields()
               .FirstOrDefault(x => TEHelpers.IsMemberMatchingToColumnName(x, fieldName));

        }

        private static PropertyInfo GetThePropertyOnThisObject(object @object, string propertyName)
        {
            var type = @object.GetType();
            return type.GetProperties()
                .FirstOrDefault(x => TEHelpers.IsMemberMatchingToColumnName(x, propertyName));
        }
    }
}