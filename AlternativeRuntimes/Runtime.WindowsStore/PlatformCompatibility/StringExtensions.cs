
namespace System
{
    public static class StringExtensions
    {
        public static bool Contains(this string self, char c)
        {
            return self.Contains(c.ToString());
        }
    }
}
