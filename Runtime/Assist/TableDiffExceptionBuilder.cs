namespace TechTalk.SpecFlow.Assist
{
    public class TableDiffExceptionBuilder<T>
    {
        public string GetTheTableDiffExceptionMessage(TableDifferenceResults<T> tableDifferenceResults)
        {
            return "  " + tableDifferenceResults.Table;
            //var realData = new StringBuilder();
            //var index = 0;
            //var everyLineInTheTable = table.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //foreach (var line in everyLineInTheTable)
            //{
            //    var prefix = "  ";
            //    if (listOfMissingItems.Contains(index))
            //        prefix = "- ";
            //    realData.AppendLine(prefix + line);
            //    index++;
            //}
            //foreach (var item in itemsThatWereNotFoundInTheTable)
            //    realData.AppendLine(string.Format("+ | {0} |", ((item.GetPropertyValue("StringProperty") as string) ?? "").PadRight(14)));
            //return realData.ToString();
        }
    }
}