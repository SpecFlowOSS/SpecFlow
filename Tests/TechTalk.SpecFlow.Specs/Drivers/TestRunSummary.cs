namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class TestRunSummary
    {
        public int Total { get; set; }
        public int Succeeded { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public int Ignored { get; set; }
    }
}