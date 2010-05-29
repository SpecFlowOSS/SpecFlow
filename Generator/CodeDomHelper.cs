using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TechTalk.SpecFlow.Generator
{
    public enum GenerationTargetLanguage
    {
        CSharp,
        VB,
        Other
    }

    public interface ICodeDomHelperRequired
    {
        CodeDomHelper CodeDomHelper { get; set; }
    }

    public class CodeDomHelper
    {
        public GenerationTargetLanguage TargetLanguage { get; private set; }

        public CodeDomHelper(CodeDomProvider codeComProvider)
        {
            switch (codeComProvider.FileExtension.ToLower(CultureInfo.InvariantCulture))
            {
                case "cs":
                    TargetLanguage = GenerationTargetLanguage.CSharp;
                    break;
                case "vb":
                    TargetLanguage = GenerationTargetLanguage.VB;
                    break;
                default:
                    TargetLanguage = GenerationTargetLanguage.Other;
                    break;
            }
        }

        public CodeDomHelper(GenerationTargetLanguage targetLanguage)
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

            if (TargetLanguage == GenerationTargetLanguage.VB)
            {
                var isInterfaceField = typeReference.GetType().GetField("isInterface",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (isInterfaceField == null)
                    throw new InvalidOperationException("CodeDom version does not support VB.NET generation.");

                isInterfaceField.SetValue(typeReference, true);
            }
        }

        public void InjectIfRequired(object target)
        {
            ICodeDomHelperRequired codeDomHelperRequired = target as ICodeDomHelperRequired;
            if (codeDomHelperRequired != null)
                codeDomHelperRequired.CodeDomHelper = this;
        }

        public void AddCommentStatement(CodeStatementCollection statements, string comment)
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement("//" + comment));
                    break;
                case GenerationTargetLanguage.VB:
                    statements.Add(new CodeSnippetStatement("'" + comment));
                    break;
            }
        }

        public void BindTypeToSourceFile(CodeTypeDeclaration typeDeclaration, string fileName)
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember(string.Format("#line 1 \"{0}\"", fileName)));
                    typeDeclaration.Members.Add(new CodeSnippetTypeMember("#line hidden"));
                    break;
            }
        }

        public void AddSourceLinePragmaStatement(CodeStatementCollection statements, int lineNo, int colNo)
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement(string.Format("#line {0}", lineNo)));
                    AddCommentStatement(statements, string.Format("#indentnext {0}", colNo - 1));
                    break;
            }
        }

        public void AddDisableSourceLinePragmaStatement(CodeStatementCollection statements)
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    statements.Add(new CodeSnippetStatement("#line hidden"));
                    break;
            }
        }

        public CodeStatement GetStartRegionStatement(string regionText)
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    return new CodeSnippetStatement("#region " + regionText);
                case GenerationTargetLanguage.VB:
                    return new CodeSnippetStatement("#Region \"" + regionText + "\"");
            }
            return new CodeCommentStatement("#region " + regionText);
        }

        public CodeStatement GetEndRegionStatement()
        {
            switch (TargetLanguage)
            {
                case GenerationTargetLanguage.CSharp:
                    return new CodeSnippetStatement("#endregion");
                case GenerationTargetLanguage.VB:
                    return new CodeSnippetStatement("#End Region");
            }
            return new CodeCommentStatement("#endregion");
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
    }
}
