SpecFlow supports testing applications written for different runtime frameworks, like [[Microsoft .NET|.NET Support]] or [[Silverlight|Silverlight Support]]. In order to achieve this, the test generator part of SpecFlow (which is usually executed from Visual Studio or MSBuild) is separated from the Runtime part (which is executing the tests in the target platform). The Generator is compiled to .NET 3.5 (also used by MonoDevelop), while the Runtime part is compiled for each supported platform separately. 

The following table contains the supported platforms.

<table>
    <tr>
        <th>Name</th>
        <th>SpecFlow runtime assembly</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>.NET 3.5<br/>.NET 4.0<br/>Mono</td>
        <td>TechTalk.SpecFlow.dll</td>
        <td>This is the primary target platform of SpecFlow. See [[.NET Support]] and [[Mono Support]].</td>
    </tr>
    <tr>
        <td>Silverlight 3<br/>Silverlight 4</td>
        <td>TechTalk.SpecFlow.Silverlight3.dll</td>
        <td>See [[Silverlight Support]].</td>
    </tr>
    <tr>
        <td>Windows Phone 7</td>
        <td>TechTalk.SpecFlow.WindowsPhone7.dll</td>
        <td>See [[Windows Phone 7 Support]].</td>
    </tr>
</table>
