using System;

namespace Extensions
{
    public static class ExtensionMethods
    {
        public static string IncludeSingleQuote(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input.Replace("'", "''");
        }

        public static long RoundOffNearestTen(this long input)
        {
            return ((long)Math.Round(input / 10.0)) * 10;
        }
    }
}
