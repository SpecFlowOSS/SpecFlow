namespace TechTalk.SpecFlow.CommonModels
{
    public class WrappedFailure : Failure
    {
        public WrappedFailure(string description, IResult innerResult) : base(description)
        {
            InnerResult = innerResult;
        }

        public IResult InnerResult { get; }
    }

    public class WrappedFailure<T> : WrappedFailure, IFailure<T>
    {
        public WrappedFailure(string description, IResult innerFailure) : base(description, innerFailure)
        {
        }
    }
}
