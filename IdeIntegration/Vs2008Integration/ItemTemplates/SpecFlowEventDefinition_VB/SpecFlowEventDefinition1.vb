Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports TechTalk.SpecFlow

Namespace $rootnamespace$

    <Binding()> _
    Public Class $safeitemname$

        <BeforeStep()> _
        Public Sub BeforeStep()
            'TODO: implement logic that has to run before each scenario step
            ' For storing and retrieving scenario-specific data, 
            ' the instance fields of the class or the
            '     ScenarioContext.Current
            ' collection can be used.
            ' For storing and retrieving feature-specific data, the 
            '     FeatureContext.Current
            ' collection can be used.
            ' Use the attribute overload to specify tags. If tags are specified, the event 
            ' handler will be executed only if any of the tags are specified for the 
            ' feature or the scenario.
            '     <BeforeStep("mytag")> _
        End Sub

        <AfterStep()> _
        Public Sub AfterStep()
            'TODO: implement logic that has to run after each scenario step
        End Sub

        <BeforeScenarioBlock()> _
        Public Sub BeforeScenarioBlock()
            'TODO: implement logic that has to run before each scenario block (given-when-then)
        End Sub

        <AfterScenarioBlock()> _
        Public Sub AfterScenarioBlock()
            'TODO: implement logic that has to run after each scenario block (given-when-then)
        End Sub

        <BeforeScenario()> _
        Public Sub BeforeScenario()
            'TODO: implement logic that has to run before executing each scenario
        End Sub

        <AfterScenario()> _
        Public Sub AfterScenario()
            'TODO: implement logic that has to run after executing each scenario
        End Sub

        <BeforeFeature()> _
        Public Shared Sub BeforeFeature()
            'TODO: implement logic that has to run before executing each feature
        End Sub

        <AfterFeature()> _
        Public Shared Sub AfterFeature()
            'TODO: implement logic that has to run after executing each feature
        End Sub

        <BeforeTestRun()> _
        Public Shared Sub BeforeTestRun()
            'TODO: implement logic that has to run before the entire test run
        End Sub

        <AfterTestRun()> _
        Public Shared Sub AfterTestRun()
            'TODO: implement logic that has to run after the entire test run
        End Sub

    End Class

End Namespace
