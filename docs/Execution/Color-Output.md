# Color Test Result Output

## Configuration

To enable the colorization of the test result output, you can turn the `trace.coloredOutput` to true in the [configuration](../Installation/Configuration.md)

The color will only be visible in supported place, like in Rider test runner or in the console when running test using `dotnet test`. 

You can turn off the color by setting `NO_COLOR=1` environment variable. This can be useful when you run the tests on a build server that does not support colors.

## Customization

You can customize the colors by configuring a Hook and injecting `IColorOutputTheme` like in the following example.

```csharp
[Binding]
public class Hooks
{
	[BeforeTestRun]
	public static void ConfigureColor(IColorOutputTheme colorOutputTheme)
	{
		colorOutputTheme.Keyword = AnsiColor.Reset;
		colorOutputTheme.Error = AnsiColor.Composite(AnsiColor.Bold, AnsiColor.Foreground(TerminalRgbColor.FromHex("FF8EF3")));
		colorOutputTheme.Done = AnsiColor.Foreground(TerminalRgbColor.FromHex("3A86FF"));
	}
}
```