namespace TechTalk.SpecFlow.CodeBehindGenerator
{
    public interface IFeatureCodeBehindGenerator
    {
        void InitializeProject(string projectPath);
        GeneratedCodeBehindFile GenerateCodeBehindFile(string featureFile);
    }
}
