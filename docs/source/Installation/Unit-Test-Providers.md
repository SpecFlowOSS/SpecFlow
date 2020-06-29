# Unit Test Providers

**Note:** This information only applies to Full Framework projects using the `app.config` file to configure SpecFlow. For .NET Core and projects using `specflow.json` to configure SpecFlow, you need to add a NuGet package to your project to determine your unit test provider, see [SpecFlow-and-.NET-Core](SpecFlow-and-.NET-Core.md),

SpecFlow supports several unit test framework you can use to execute your tests. In addition to using the built-in unit test providers, you can also create a custom provider. Use the [&lt;unitTestProvider&gt;](../Configuration/Configuration.md) configuration element in your `app.config` file to specify which unit test provider you want to use.

The following table contains the built-in unit test providers.

<table>
    <tr>
        <th rowspan="2">Name</th>
        <th colspan="3">Supports</th>
        <th rowspan="2">Description</th>
    </tr>
    <tr>
        <th>row tests</th>
        <th>categories</th>
        <th>inconclusive</th>
    </tr>
    <tr>
        <td>SpecFlow+ Runner (fka SpecRun)</td>
        <td>+</td>
        <td>+</td>
        <td>+</td>
        <td>[SpecFlow+ Runner](http://www.specflow.org/plus/runner/) is a dedicated test execution framework for SpecFlow. Install it with the SpecRun.SpecFlow NuGet package. See [SpecRun Integration]() for details.</td>
    </tr>
    <tr>
        <td>NUnit</td>
        <td>+</td>
        <td>+</td>
        <td>+</td>
        <td>See [http://www.nunit.org](http://www.nunit.org). Specialized [NuGet packages](NuGet-Packages.md) available for easy setup: SpecFlow.NUnit, SpecFlow.NUnit.Runners. Supports parallel execution with NUnit v3. </td>
    </tr>
    <tr>
        <td>MsTest.2008</td>
        <td>-</td>
        <td>-</td>
        <td>+</td>
        <td>MsTest provider for .NET 3.5</td>
    </tr>
    <tr>
        <td>MsTest <br/> MsTest.2010</td>
        <td>-</td>
        <td>+</td>
        <td>+</td>
        <td>MsTest provider for .NET 4.0. Supports test categories. Specialized [NuGet package|NuGet Integration]() available for easy setup: SpecFlow.MsTest.</td>
    </tr>
    <tr>
        <td>xUnit</td>
        <td>+</td>
        <td>-</td>
        <td>-</td>
        <td>See [http://www.xunit.net](). Specialized [NuGet package|NuGet Integration]() available for easy setup: SpecFlow.xUnit. Supports parallel execution with xUnit v2.</td>    
    </tr>
</table>
