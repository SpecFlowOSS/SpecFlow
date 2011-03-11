namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface INativeSuggestionItemFactory<TNativeSuggestionItem>
    {
        TNativeSuggestionItem Create(string displayText, string insertionText, int level);
        string GetInsertionText(TNativeSuggestionItem nativeSuggestionItem);
    }
}