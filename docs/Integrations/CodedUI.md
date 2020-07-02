# Coded UI

## Introduction

**Note: Coded UI is no longer supported with SpecFlow 3.** We recommend using Appium or WinAppDriver instead. The following is preserved for legacy users.

The Microsoft Coded UI API can be used to create automated tests in Visual Studio, but is not directly compatible with SpecFlow as each Test Class requires the `[CodedUITest]` attribute, which SpecFlow does not generate by default.

Big thanks go to Thomy Kay for [pointing us in the right direction](http://groups.google.com/group/specflow/browse_thread/thread/e162fc98c1d7c119/0bf231a65195b375?lnk=gst&q=SpecFlow+with+VS2010+CodedUI+tests+#0bf231a65195b375).

## Solution

You need to ensure SpecFlow generates the `[CodedUITest]` attribute by creating a custom test generator provider, copying the DLL file into the `tools` directory where the SpecFlow NuGet package is installed, and ensure that any SpecFlow hooks also ensure the CodedUI API is initialized.

### Generating the [CodedUITest] attribute with VS2010 and MSTest

1. Create a new VS project to generate an assembly that contains the class below.
This will require a reference to `TechTalk.SpecFlow.Generator.dll` in the SpecFlow directory. If you are using version 1.7 or higher you will also need to add a reference to `TechTalk.SpecFlow.Utils.dll`
2. Add the following class to your new VS project:

**SpecFlow version 1.6**

```csharp
namespace My.SpecFlow
{
    using System.CodeDom;
    using TechTalk.SpecFlow.Generator.UnitTestProvider;

    public class MsTest2010CodedUiGeneratorProvider : MsTest2010GeneratorProvider
    {
        public override void SetTestFixture(System.CodeDom.CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            base.SetTestFixture(typeDeclaration, title, description);
            foreach (CodeAttributeDeclaration customAttribute in typeDeclaration.CustomAttributes)
            {
                if (customAttribute.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")
                {
                    typeDeclaration.CustomAttributes.Remove(customAttribute);
                    break;
                }
            }

            typeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute")));
        }
    } 
}
```
**SpecFlow version 1.7**

```csharp
using System.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace SpecflowCodedUIGenerator
{
    public class MsTest2010CodedUiGeneratorProvider : MsTest2010GeneratorProvider
    {
        public override void SetTestClass(TechTalk.SpecFlow.Generator.TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            base.SetTestClass(generationContext, featureTitle, featureDescription);

            foreach (CodeAttributeDeclaration customAttribute in generationContext.TestClass.CustomAttributes)
            {
                if (customAttribute.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")
                {
                    generationContext.TestClass.CustomAttributes.Remove(customAttribute);
                    break;
                }
            }

            generationContext.TestClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute")));
        }
    }
} 
```
**SpecFlow version 1.9**

```csharp
using System.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace SpecflowCodedUIGenerator
{
     public class MsTest2010CodedUiGeneratorProvider : MsTest2010GeneratorProvider
    {
        public MsTest2010CodedUiGeneratorProvider(CodeDomHelper codeDomHelper)
            : base(codeDomHelper)
        {
        }

        public override void SetTestClass(TechTalk.SpecFlow.Generator.TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            base.SetTestClass(generationContext, featureTitle, featureDescription);

            foreach (CodeAttributeDeclaration customAttribute in generationContext.TestClass.CustomAttributes)
            {
                if (customAttribute.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")
                {
                    generationContext.TestClass.CustomAttributes.Remove(customAttribute);
                    break;
                }
            }

            generationContext.TestClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute")));
        }
    }
} 
```

3. Build the project to generate an assembly (.dll) file. Make sure this is built against the same version of the .NET as SpecFlow, and copy this file to your SpecFlow installation directory.

4. Add a config item to your CodedUI project's `App.Config` file

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="specFlow"
    type="TechTalk.SpecFlow.Configuration.ConfigurationSectionHandler,
TechTalk.SpecFlow"/>
  </configSections>
  <specFlow>
    <unitTestProvider name="MsTest.2010"
  generatorProvider="My.SpecFlow.MsTest2010CodedUiGeneratorProvider,
My.SpecFlow"
  runtimeProvider="TechTalk.SpecFlow.UnitTestProvider.MsTest2010RuntimeProvider,
TechTalk.SpecFlow"/>
  </specFlow>
</configuration>
```

5. Now when you generate a new feature file, it will add the appropriate attributes.

### Getting SpecFlow to generate the [CodedUITest] attribute with Visual Studio 2013+ and MSTest

1. Create a new Class Library project in Visual Studio (example: `TechTalk.SpecFlow.CodedUI.MsTest`).

2. Install the `SpecFlow` NuGet package via the Package Manager Console.

3. Create a new Class called `SpecFlowCodedUITestGenerator`.

  1. Right click the Project in the Solution Explorer pane.
  2. Click "Add..." then click "References...".

  3. Add a reference to the following DLLs:
    - `<Solution Directory>\packages\SpecFlow.X.Y.Z\tools\TechTalk.SpecFlow.Generator.dll`
    - `<Solution Directory>\packages\SpecFlow.X.Y.Z\tools\TechTalk.SpecFlow.Utils.dll`
  
  4. Copy the following code to the  `SpecFlowCodedUITestGenerator` class:
    
  ```csharp
  using System.CodeDom;
  using TechTalk.SpecFlow.Generator.UnitTestProvider;
  using TechTalk.SpecFlow.Utils;

  namespace TechTalk.SpecFlow.CodedUI.MsTest
  {
      public class SpecFlowCodedUITestGenerator : MsTestGeneratorProvider
      {
          public SpecFlowCodedUITestGenerator(CodeDomHelper codeDomHelper) : base(codeDomHelper)
          {
          }

          public override void SetTestClass(TechTalk.SpecFlow.Generator.TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
          {
              base.SetTestClass(generationContext, featureTitle, featureDescription);

              foreach (CodeAttributeDeclaration customAttribute in generationContext.TestClass.CustomAttributes)
              {
                  if (customAttribute.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute")
                  {
                      generationContext.TestClass.CustomAttributes.Remove(customAttribute);
                      break;
                  }
              }

              generationContext.TestClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("Microsoft.VisualStudio.TestTools.UITesting.CodedUITestAttribute")));
          }
      }
  }
  ```

  5. Right-click the Project in the Solution Explorer pane, and click "Properties".

  6. Go to the "Build Events" tab.

  7. In the "Post-build event command line" box, enter the following command:

  ```
  copy $(TargetPath) $(SolutionDir)packages\SpecFlow.X.Y.Z\tools\
  ```

    **Important!** The DLL created by building the `TechTalk.SpecFlow.CodedUI.MsTest` project needs to be copied to the `packages\SpecFlow.X.Y.Z\tools` directory of the Visual Studio solution that contains your SpecFlow tests in order for this to work.

4. In the "Properties" for the `TechTalk.SpecFlow.CodedUI.MsTest` project, go to the "Application" tab
5. Choose ".NET Framework 3.5" for SpecFlow 1.9 or ".NET Framework 4.5" for SpecFlow 2.0+ in the "Target framework" drop-down.
6. Change the `<unitTestProvider>` in App.config to use the new test generator:

  ```xml
  <?xml version="1.0" encoding="utf-8"?>
  <configuration>
    <specFlow>
      <unitTestProvider name="MsTest"
        generatorProvider="TechTalk.SpecFlow.CodedUI.MsTest.SpecFlowCodedUITestGenerator, TechTalk.SpecFlow.CodedUI.MsTest"
        runtimeProvider="TechTalk.SpecFlow.UnitTestProvider.MsTestRuntimeProvider, TechTalk.SpecFlow" />
    </specFlow>
  </configuration>
  ```
  
  If Visual Studio prompts you to regenerate the feature files, do so. If not, right-click on the project containing your SpecFlow tests and click "Regenerate Feature Files".

### Ensuring the SpecFlow hooks can use the CodedUI API

If you want to use any of the SpecFlow Hooks as steps such as [BeforeTestRun],[BeforeFeature], [BeforeScenario], [AfterTestRun], [AfterFeature] or [AfterScenario], you will receive the following error:
`Microsoft.VisualStudio.TestTools.UITest.Extension.TechnologyNotSupportedException: The browser  is currently not supported`

Solve this by adding a `Playback.Initialize();` call in your [BeforeTestRun] step, and a `Playback.Cleanup();` in your [AfterTestRun] step.

## Other Information
[Blog series on using Specflow with Coded UI Test API] (http://rburnham.wordpress.com/2011/03/15/bdd-ui-automation-with-specflow-and-coded-ui-tests/)