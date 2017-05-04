using TechTalk.SpecFlow.Specs.Drivers;

namespace TechTalk.SpecFlow.Specs.StepDefinitions
{
    [Binding]
    public sealed class ContentFileSteps
    {
        private readonly InputProjectDriver inputProjectDriver;

        public ContentFileSteps(InputProjectDriver inputProjectDriver)
        {
            this.inputProjectDriver = inputProjectDriver;
        }

        [Given("there is a content file '(.*)' in the project as")]
        public void GivenThereIsAContentFileInTheProjectAs(string fileName, string fileContent)
        {
            inputProjectDriver.AddContentFile(fileName, fileContent);
        }
    }
}
