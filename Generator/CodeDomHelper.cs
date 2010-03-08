using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TechTalk.SpecFlow.Generator
{
    internal static class CodeDomHelper
    {
        static public CodeTypeReference CreateNestedTypeReference(CodeTypeDeclaration baseTypeDeclaration, string nestedTypeName)
        {
            return new CodeTypeReference(baseTypeDeclaration.Name + "." + nestedTypeName);
        }

        static public void SetTypeReferenceAsInterface(CodeTypeReference typeReference)
        {
            // this hack is necessary for VB.NET code generation

            var isInterfaceField = typeReference.GetType().GetField("isInterface", BindingFlags.Instance | BindingFlags.NonPublic);
            if (isInterfaceField != null)
                isInterfaceField.SetValue(typeReference, true);
        }
    }
}
