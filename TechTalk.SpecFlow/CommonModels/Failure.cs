namespace TechTalk.SpecFlow.CommonModels
{
    public class Failure : IFailure
    {
        public Failure(string description)
        {
            Description = description;
        }

        public string Description { get; }

        public override string ToString() => Description;
    }

    public class Failure<T> : Failure, IFailure<T>
    {
        public Failure(string description) : base(description)
        {
        }
    }
}
