using System.Globalization;
using System.Text.RegularExpressions;

namespace DapperOperations.Extensions
{
    internal static class StringExtension
    {
        /// <summary>
        /// Convert string to Snake Case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string ToPascalCase(this string input)
        {
            var yourString = input.ToLower().Replace("_", " ");
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(yourString).Replace(" ", string.Empty);
        }

        public static string ToCamelCase(this string input)
        {
            if (!string.IsNullOrEmpty(input) && input.Length > 1)
            {
                return char.ToLowerInvariant(input[0]) + input.Substring(1);
            }
            return input;
        }

        public static string FormatByConvetion(this string input)
        {
            return DapperOperation.NameConvetion switch
            {
                NameConvetion.PascalCase => input.ToPascalCase(),
                NameConvetion.SnakeCase => input.ToSnakeCase(),
                NameConvetion.CamelCase => input.ToCamelCase(),
                _ => input,
            };
        }
    }
}
