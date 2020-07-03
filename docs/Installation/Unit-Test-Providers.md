# Unit Test Providers

SpecFlow supports several unit test framework you can use to execute your tests.  

To use a specific unit test provider, you have to add it's dedicated NuGet package to your project.  
You can only have one of these packages added to your project at once.

| Name | NuGet Package | Integration | Description |
| -----|---------------|-------------|-------------|
| SpecFlow+ Runner (fka SpecRun) | [SpecRun.SpecFlow](https://www.nuget.org/packages/SpecRun.SpecFlow/) | [goto Integration](../Integrations/SpecFlow+Runner-Integration.html) | [SpecFlow+ Runner](http://www.specflow.org/plus/runner/) is a dedicated test execution framework for SpecFlow. |
| xUnit | [SpecFlow.xUnit](https://www.nuget.org/packages/SpecFlow.xUnit/) | [goto Integration](../Integrations/xUnit.html) | See <http://www.xunit.net> |
| NUnit | [SpecFlow.NUnit](https://www.nuget.org/packages/SpecFlow.NUnit/) | [goto Integration](../Integrations/NUnit.html) | See <http://www.nunit.org> |
| MSTest | [SpecFlow.MsTest](https://www.nuget.org/packages/SpecFlow.MsTest/) | [goto Integration](../Integrations/MsTest.html) | See <https://github.com/microsoft/testfx>|
