using System;
using System.Collections.Generic;
using System.Linq;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace TechTalk.SpecFlow.Utils
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
                var isInterfaceField = typeReference.GetType().GetField("isInterface",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (isInterfaceField == null)
                    throw new InvalidOperationException("CodeDom version does not support VB.NET generation.");

                isInterfaceField.SetValue(typeReference, true);
            }
        }

        public void AddCommentStatement(CodeStatementCollection statements, string comment)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement("//" + comment));
                    break;
                case CodeDomProviderLanguage.VB:
                    statements.Add(new CodeSnippetStatement("'" + comment));
                    break;
            }
        }

        private bool isExternalSourceBlockOpen = false;
        private string currentBoundSourceCodeFile = null;

        public void BindTypeToSourceFile(CodeTypeDeclaration typeDeclaration, string fileName)
        {
            currentBoundSourceCodeFile = fileName;

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

        public void AddSourceLinePragmaStatement(CodeStatementCollection statements, int lineNo, int colNo)
        {
            if (currentBoundSourceCodeFile == null)
                throw new InvalidOperationException("The generated code was not bound to a file!");

            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.VB:
                    if (isExternalSourceBlockOpen)
                        AddDisableSourceLinePragmaStatement(statements);
                    statements.Add(new CodeSnippetStatement(string.Format("#ExternalSource(\"{0}\",{1})", currentBoundSourceCodeFile, lineNo)));
                    isExternalSourceBlockOpen = true;
                    AddCommentStatement(statements, string.Format("#indentnext {0}", colNo - 1));
                    break;
                case CodeDomProviderLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement(string.Format("#line {0}", lineNo)));
                    AddCommentStatement(statements, string.Format("#indentnext {0}", colNo - 1));
                    break;
            }
        }

        public void AddDisableSourceLinePragmaStatement(CodeStatementCollection statements)
        {
            switch (TargetLanguage)
            {
                case CodeDomProviderLanguage.VB:
                    statements.Add(new CodeSnippetStatement("#End ExternalSource"));
                    isExternalSourceBlockOpen = false;
                    break;
                case CodeDomProviderLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement("#line hidden"));
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

        public CodeAttributeDeclaration AddAttribute(CodeTypeMember codeTypeMember, string attrType, params CodeAttributeArgument[] attrArgumets)
        {
            var codeAttributeDeclaration = new CodeAttributeDeclaration(attrType, attrArgumets);
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
    }
}
