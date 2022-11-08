using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Bindings.Discovery
{
    public interface IRuntimeBindingSourceProcessor : IBindingSourceProcessor
    {
        void RegisterTypeLoadError(string errorMessage);
    }

    public class RuntimeBindingSourceProcessor : BindingSourceProcessor, IRuntimeBindingSourceProcessor
    {
        private readonly IBindingRegistry _bindingRegistry;
        private readonly ITestTracer _testTracer;

        public RuntimeBindingSourceProcessor(IBindingFactory bindingFactory, IBindingRegistry bindingRegistry, ITestTracer testTracer) : base(bindingFactory)
        {
            _bindingRegistry = bindingRegistry;
            _testTracer = testTracer;
        }

        protected override void ProcessStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            _bindingRegistry.RegisterStepDefinitionBinding(stepDefinitionBinding);
        }

        protected override void ProcessHookBinding(IHookBinding hookBinding)
        {
            _bindingRegistry.RegisterHookBinding(hookBinding);
        }

        protected override void ProcessStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            _bindingRegistry.RegisterStepArgumentTransformationBinding(stepArgumentTransformationBinding);
        }

        protected override void OnValidationError(BindingValidationResult validationResult, bool genericBindingError)
        {
            if (validationResult.IsValid)
                return;

            foreach (string errorMessage in validationResult.ErrorMessages)
            {
                _testTracer.TraceWarning($"Invalid binding: {errorMessage}");
                if (genericBindingError)
                    _bindingRegistry.RegisterGenericBindingError(new BindingError(BindingErrorType.BindingError, errorMessage));
            }

            base.OnValidationError(validationResult, genericBindingError);
        }

        public override void BuildingCompleted()
        {
            base.BuildingCompleted();
            _bindingRegistry.Ready = true;
        }

        public void RegisterTypeLoadError(string errorMessage)
        {
            _bindingRegistry.RegisterGenericBindingError(new BindingError(BindingErrorType.TypeLoadError, errorMessage));
        }
    }
}
