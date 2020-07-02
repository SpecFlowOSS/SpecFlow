# Parallel Execution

SpecFlow is mainly used to drive integration test that have external dependencies and applications with complex internal architecture. Because of this, it is generally not easy to execute these tests in parallel.

**Note**: This page only covers parallel execution with SpecFlow. For options when executing tests with SpecFlow+ Runner, refer to the [SpecFlow+ documentation](https://specflow.org/plus/documentation/Execution/)

## Parallel Execution with Memory (AppDomain) Isolation

If there are no external dependencies or they can be cloned for parallel execution, but the application architecture depends on a static state (e.g. a caches etc.), the best way is to execute tests in parallel isolated by AppDomain. This ensures that every test execution thread is hosted in a separate AppDomain and that each thread's memory (e.g. static fields) is isolated. In such scenarios, SpecFlow can be used to execute tests in parallel without any extra considerations. SpecFlow+ Runner supports parallel execution with AppDomain, SharedAppDomain and Process isolation.

**Note:** The `[BeforeTestRun]` and `[AfterTestRun]` hooks are executed for each individual test execution thread (AppDomain), so you can use them to initialize/reset shared memory.  

## Parallel Execution Without Memory Isolation

If your tests do not depend on any static states (i.e. do not store any test-specific information in static fields), you can run the tests in parallel without AppDomain isolation. Executing tests this way has a smaller initialization footprint and lower memory requirements.

To run SpecFlow tests in parallel without memory isolation, the following requirements must be met:

* You must be using a test runner that supports this feature (currently NUnit v3, xUnit v2, MSTest and SpecFlow+ Runner)
* You may not be using the static context properties `ScenarioContext.Current`, `FeatureContext.Current` or `ScenarioStepContext.Current` (see examples below).
 
### Execution Behaviour

* `[BeforeTestRun]` and `[AfterTestRun]` hooks (events) are only executed once, on the first thread that initializes the framework. Executing tests in the other threads is blocked until the hooks have been fully executed on the first thread.
* Each thread manages its own enter/exit feature execution workflow. The `[BeforeFeature]` and `[AfterFeature]` hooks may be executed multiple times in different threads if the different threads run scenarios from the same feature file. The execution of these hooks do not block one another, but the Before/After feature hooks are called in pairs within a single thread (the `[BeforeFeature]` hook of the next scenario is only executed after the `[AfterFeature]` hook of the previous one). Each thread has a separate (and isolated) `FeatureContext`.
* Scenarios and their related hooks (Before/After scenario, scenario block, step) are isolated in the different threads during execution and do not block each other. Each thread has a separate (and isolated) `ScenarioContext`.
* All scenarios in a feature are executed on the **same thread**.
* The test trace listener (that outputs the scenario execution trace to the console by default) is invoked asynchronously from the multiple threads and the trace messages are queued and passed to the listener in serialized form. If the test trace listener implements `TechTalk.SpecFlow.Tracing.IThreadSafeTraceListener`, the messages are sent directly from the threads. 
* The binding registry (that holds the step definitions, hooks, etc.) and some other core services are shared across test threads.

### Notes for NUnit v3 Support

The NUnit v3 unit test provider (`nunit`) does not currently generate `[Parallelizable]` attributes on feature classes or scenario methods. Parallelisation must be configured by setting an assembly-level attribute in the SpecFlow project.

```c#
[assembly: Parallelizable(ParallelScope.Fixtures)]
```

### Thread-safe ScenarioContext, FeatureContext and ScenarioStepContext

Context injection is a type safe state sharing method that is thread-safe, and is also recommended for non-parallel execution scenarios. 

When using parallel execution with generic contexts, the context classes have to be injected to the binding class instead of accessing the `ScenarioContext.Current`, `FeatureContext.Current` or `ScenarioStepContext.Current` static properties, or the instance properties of the `Steps` base class can be used. Accessing the static properties during parallel execution throws a `SpecFlowException`.

#### Injecting ScenarioContext to the binding class

```c#
[Binding]
public class StepsWithScenarioContext
{
	private readonly ScenarioContext scenarioContext;

	public StepsWithScenarioContext(ScenarioContext scenarioContext)
	{
		if (scenarioContext == null) throw new ArgumentNullException("scenarioContext");
		this.scenarioContext = scenarioContext;
	}

	[Given(@"I put something into the context")]
	public void GivenIPutSomethingIntoTheContext()
	{
		scenarioContext.Set("test-value", "test-key");
	}
}
```

You can inject `FeatureContext` in a similar manner, and use the `StepContext` property of the injected `ScenarioContext` to access the `ScenarioStepContext`.

#### Using ScenarioContext from the Steps Base Class

```c#
[Binding]
public class StepsWithScenarioContext : Steps
{
	[Given(@"I put something into the context")]
	public void GivenIPutSomethingIntoTheContext()
	{
		this.ScenarioContext.Set("test-value", "test-key");
	}
}
```

The other contexts can be accessed with the `FeatureContext` and the `StepContext` properties.