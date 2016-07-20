using System;

namespace TechTalk.SpecFlow.RuntimeTests
{
  public static class TestHelpers
  {
    public static string AgnosticLineBreak(this String theString)
    {
        return theString.Replace("\r\n", "\n");
    }
  } 
}
