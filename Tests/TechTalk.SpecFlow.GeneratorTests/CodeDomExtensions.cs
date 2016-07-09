using System.CodeDom;
using System.Collections.Generic;
using FluentAssertions;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public static class CodeDomExtensions
    {
        public static CodeTypeDeclaration Class(this CodeNamespace code)
        {
            return code.Types[0];
        }

        public static IEnumerable<CodeMemberMethod> Members(this CodeTypeDeclaration codeTypeDeclaration)
        {
            foreach (var member in codeTypeDeclaration.Members)
            {
                var method = member as CodeMemberMethod;
                if (method != null)
                {
                    yield return method;
                }
            }
        }

        public static IEnumerable<CodeAttributeDeclaration> CustomAttributes(this CodeMemberMethod codeMemberMethod)
        {
            foreach (var customAttribute in codeMemberMethod.CustomAttributes)
            {

                var attribute = customAttribute as CodeAttributeDeclaration;
                if (attribute != null)
                {
                    yield return attribute;
                }
            }
        }

        public static IEnumerable<object> ArgumentValues(this CodeAttributeDeclaration attribute)
        {
            foreach (CodeAttributeArgument arguments in attribute.Arguments)
            {
                yield return arguments.Value.As<CodePrimitiveExpression>().Value;
            }
        }

        public static IEnumerable<CodeAttributeDeclaration> CustomAttributes(this CodeTypeDeclaration codeTypeDeclaration)
        {
            foreach (var customAttribute in codeTypeDeclaration.CustomAttributes)
            {

                var attribute = customAttribute as CodeAttributeDeclaration;
                if (attribute != null)
                {
                    yield return attribute;
                }
            }
        }
    }
}