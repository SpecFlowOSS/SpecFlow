using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace TechTalk.SpecFlow.Generator.CodeDom
{
    public class CodeDomHelper
    {
        public CodeDomProviderLanguage TargetLanguage { get; private set; }

        public CodeDomHelper(CodeDomProvider codeComProvider)
        {
            switch (codeComProvider.FileExtension.ToLower(CultureInfo.InvariantCulture))
            {
                case "cs":
                    TargetLanguage = CodeDomProviderLanguage.CSharp;
                    break;
                case "vb":
                    TargetLanguage = CodeDomProviderLanguage.VB;
                    break;
                default:
                    TargetLanguage = CodeDomProviderLanguage.Other;
                    break;
            }
        }

        public CodeDomHelper(CodeDomProviderLanguage targetLanguage)
        {
            TargetLanguage = targetLanguage;
        }

        public CodeTypeReference CreateNestedTypeReference(CodeTypeDeclaration baseTypeDeclaration, string nestedTypeName)
        {
            return new CodeTypeReference(baseTypeDeclaration.Name + "." + nestedTypeName);
        }

        public void SetTypeReferenceAsInterface(CodeTypeReference typeReference)
        {
            // this hack is necessary for VB.NET code generation
            if (TargetLanguage == CodeDomProviderLanguage.VB)
            {
                var typeReferenceType = typeReference.GetType();
                var isInterfaceField = typeReferenceType.GetField("_isInterface", BindingFlags.Instance | BindingFlags.NonPublic);
                if (isInterfaceField == null)
                {
                    throw new InvalidOperationException("CodeDom version does not support VB.NET generation.");
                }

                isInterfaceField.SetValue(typeReference, true);
            }
        }

        private CodeStatement CreateCommentStatement(string comment)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("//" + comment);
                case CodeDomProviderLanguage.VB:
                    return new CodeSnippetStatement("'" + comment);
            }

            throw TargetLanguageNotSupportedException();
        }

        private NotImplementedException TargetLanguageNotSupportedException()
        {
            return new NotImplementedException($"{TargetLanguage} is not supported");
        }


        public void BindTypeToSourceFile(CodeTypeDeclaration typeDeclaration, string fileName)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.VB:
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember(string.Format("#ExternalSource(\"{0}\",1)", fileName)));
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember("#End ExternalSource"));
                    break;

                case CodeDomProviderLanguage.CSharp:
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember(string.Format("#line 1 \"{0}\"", fileName)));
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember("#line hidden"));
                    break;
            }
        }


        public CodeStatement GetStartRegionStatement(string regionText)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("#region " + regionText);
                case CodeDomProviderLanguage.VB:
                    return new CodeSnippetStatement("#Region \"" + regionText + "\"");
            }
            return new CodeCommentStatement("#region " + regionText);
        }

        public CodeStatement GetEndRegionStatement()
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("#endregion");
                case CodeDomProviderLanguage.VB:
                    return new CodeSnippetStatement("#End Region");
            }
            return new CodeCommentStatement("#endregion");
        }


        public CodeStatement GetDisableWarningsPragma()
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("#pragma warning disable");
                case CodeDomProviderLanguage.VB:
                    return new CodeCommentStatement("#pragma warning disable"); //not supported in VB
            }
            return new CodeCommentStatement("#pragma warning disable");
        }

        public CodeStatement GetEnableWarningsPragma()
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("#pragma warning restore");
                case CodeDomProviderLanguage.VB:
                    return new CodeCommentStatement("#pragma warning restore"); //not supported in VB
            }
            return new CodeCommentStatement("#pragma warning restore");
        }

        private Version GetCurrentSpecFlowVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public CodeTypeDeclaration CreateGeneratedTypeDeclaration(string className)
        {
            var result = new CodeTypeDeclaration(className);
            result.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(GeneratedCodeAttribute)),
                    new CodeAttributeArgument(new CodePrimitiveExpression("TechTalk.SpecFlow")),
                    new CodeAttributeArgument(new CodePrimitiveExpression(GetCurrentSpecFlowVersion().ToString()))));
            result.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(typeof(CompilerGeneratedAttribute))));

            return result;
        }

        public string GetErrorStatementString(string msg)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return "#error " + msg;
                default:
                    return msg;
            }
        }

        public CodeAttributeDeclaration AddAttribute(CodeTypeMember codeTypeMember, string attrType)
        {
            var codeAttributeDeclaration = new CodeAttributeDeclaration(attrType);
            codeTypeMember.CustomAttributes.Add(codeAttributeDeclaration);
            return codeAttributeDeclaration;
        }

        public CodeAttributeDeclaration AddAttribute(CodeTypeMember codeTypeMember, string attrType, params object[] attrValues)
        {
            var codeAttributeDeclaration = new CodeAttributeDeclaration(attrType,
                attrValues.Select(attrValue => new CodeAttributeArgument(new CodePrimitiveExpression(attrValue))).ToArray());
            codeTypeMember.CustomAttributes.Add(codeAttributeDeclaration);
            return codeAttributeDeclaration;
        }

        public CodeAttributeDeclaration AddAttribute(CodeTypeMember codeTypeMember, string attrType, params CodeAttributeArgument[] attrArguments)
        {
            var codeAttributeDeclaration = new CodeAttributeDeclaration(attrType, attrArguments);
            codeTypeMember.CustomAttributes.Add(codeAttributeDeclaration);
            return codeAttributeDeclaration;
        }

        public void AddAttributeForEachValue<TValue>(CodeTypeMember codeTypeMember, string attrType, IEnumerable<TValue> attrValues)
        {
            foreach (var attrValue in attrValues)
                AddAttribute(codeTypeMember, attrType, attrValue);
        }

        public CodeDomProvider CreateCodeDomProvider()
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    return new CSharpCodeProvider();
                case CodeDomProviderLanguage.VB:
                    return new VBCodeProvider();
                default:
                    throw new NotSupportedException();
            }
        }

        public CodeMemberMethod CreateMethod(CodeTypeDeclaration type)
        {
            var method = new CodeMemberMethod();
            type.Members.Add(method);
            return method;
        }

        public CodeStatement CreateDisableSourceLinePragmaStatement()
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.VB:
                    return new CodeSnippetStatement("#End ExternalSource");
                case CodeDomProviderLanguage.CSharp:
                    return new CodeSnippetStatement("#line hidden");
            }

            throw TargetLanguageNotSupportedException();
        }

        public IEnumerable<CodeStatement> CreateSourceLinePragmaStatement(string filename, int lineNo, int colNo)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.VB:
                    yield return new CodeSnippetStatement($"#ExternalSource(\"{filename}\",{lineNo})");
                    yield return CreateCommentStatement($"#indentnext {colNo - 1}");
                    break;
                case CodeDomProviderLanguage.CSharp:
                    yield return new CodeSnippetStatement($"#line {lineNo}");
                    yield return CreateCommentStatement($"#indentnext {colNo - 1}");
                    break;
            }
        }
    }
}


