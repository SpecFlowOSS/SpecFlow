namespace TechTalk.SpecFlow.Assist
{
    /// <summary>
    /// A class that will compare a key->value from a table to an actual value.
    /// </summary>
    public interface IValueComparer
    {
        /// <summary>
        /// Determines if this comparer can compare the actual value to a key->value set defined in a table.
        /// </summary>
        /// <returns><c>true</c> if this instance can compare the value to a key->value set in a table; otherwise, <c>false</c>.</returns>
        /// <param name="actualValue">Actual value.</param>
        bool CanCompare(object actualValue);

        /// <summary>
        /// Compare the expected value to the actual value.
        /// </summary>
        /// <param name="expectedValue">Expected value.</param>
        /// <param name="actualValue">Actual value.</param>
        bool Compare(string expectedValue, object actualValue);
    }
}