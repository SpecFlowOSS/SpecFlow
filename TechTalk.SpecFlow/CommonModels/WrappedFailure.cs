namespace TechTalk.SpecFlow.CommonModels
{
    public class WrappedFailure : Failure
    {
        public WrappedFailure(string description, IFailure innerFailure) : base(description)
        {
            InnerFailure = innerFailure;
        }

        public IFailure InnerFailure { get; }

        public string GetStringOfInnerFailure()
        {
            switch (InnerFailure)
            {
                case WrappedFailure wrappedFailure: return wrappedFailure.ToString();
                case Failure failure: return failure.Description;
                default: return InnerFailure?.ToString();
            }
        }

        public override string ToString()
        {
            return $"{Description}; {GetStringOfInnerFailure()}";
        }
    }

    public class WrappedFailure<T> : WrappedFailure, IFailure<T>
    {
        public WrappedFailure(string description, IFailure innerFailure) : base(description, innerFailure)
        {
        }
    }
}
