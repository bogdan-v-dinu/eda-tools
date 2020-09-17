using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CloudEventsDemo.Serialization
{
    public static class Utils
    {
        /// <summary>
        /// shoud verify. source https://stackoverflow.com/questions/37301287/how-do-i-convert-pascalcase-to-kebab-case-with-c
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string PascalToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                value,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }

        public static string ConvertPascalToKebabCase(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.PascalToKebabCase();
        }

    }
}
