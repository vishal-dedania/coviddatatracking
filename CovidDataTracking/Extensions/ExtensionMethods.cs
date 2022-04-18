namespace Extensions
{
    public static class ExtensionMethods
    {
        public static string IncludeSingleQuote(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input.Replace("'", "''");
        }
    }
}
