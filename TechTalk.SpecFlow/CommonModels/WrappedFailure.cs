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
            return InnerFailure switch
            {
                WrappedFailure wrappedFailure => wrappedFailure.ToString(),
                Failure failure => failure.Description,
                _ => InnerFailure?.ToString()
            };
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
