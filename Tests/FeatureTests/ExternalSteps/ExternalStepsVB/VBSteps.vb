Imports TechTalk.SpecFlow

<Binding()> _
Public Class VBStepDefinitions

    <Given("I have external step definitions in a separate assembly referenced by this project")> _
    Public Sub step1()
        ScenarioContext.Current.Item("counter") = 1
    End Sub

    <[When]("I call those steps")> _
    Public Sub step2()
        ScenarioContext.Current.Item("counter") += 1
    End Sub

End Class
