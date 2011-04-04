using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public interface ITestUpToDateChecker
    {
        bool? IsUpToDatePreliminary(FeatureFileInput featureFileInput, string generatedTestFullPath, UpToDateCheckingMethod upToDateCheckingMethod);
        bool IsUpToDate(FeatureFileInput featureFileInput, string generatedTestFullPath, string generatedTestContent, UpToDateCheckingMethod upToDateCheckingMethod);
    }
}
