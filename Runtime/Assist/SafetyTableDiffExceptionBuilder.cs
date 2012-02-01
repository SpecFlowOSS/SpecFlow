namespace TechTalk.SpecFlow.Assist
{
    internal class SafetyTableDiffExceptionBuilder<T> : ITableDiffExceptionBuilder<T>
    {
        private readonly ITableDiffExceptionBuilder<T> tableDiffExceptionBuilder;

        public SafetyTableDiffExceptionBuilder(ITableDiffExceptionBuilder<T> tableDiffExceptionBuilder)
        {
            this.tableDiffExceptionBuilder = tableDiffExceptionBuilder;
        }

        public string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults)
        {
            try
            {
                return tableDiffExceptionBuilder.GetTheTableDiffExceptionMessage(tableDifferenceResults);
            }
            catch
            {
                return "The table and the set not match.";
            }
        }
    }
}