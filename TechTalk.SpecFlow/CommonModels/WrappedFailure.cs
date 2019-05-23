namespace TechTalk.SpecFlow.CommonModels
{
    public class WrappedFailure : Failure
    {
        public WrappedFailure(string description, IFailure innerFailure) : base(description)
        {
            InnerFailure = innerFailure;
        }

        public IFailure InnerFailure { get; }
    }

    public class WrappedFailure<T> : WrappedFailure, IFailure<T>
    {
        public WrappedFailure(string description, IFailure innerFailure) : base(description, innerFailure)
        {
        }
    }
}
