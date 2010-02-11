Imports TechTalk.SpecFlow

<Binding()> _
Public Class VBStepDefinitions

    <Given("I have external step definitions in a separate assembly referenced by this project")> _
    Public Function step1()
        ScenarioContext.Current.Item("counter") = 1
    End Function

    <[When]("I call those steps")> _
    Public Function step2()
        ScenarioContext.Current.Item("counter") += 1
    End Function

End Class
