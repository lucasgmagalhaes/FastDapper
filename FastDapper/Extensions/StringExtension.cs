using System.Globalization;
using System.Text.RegularExpressions;

namespace FastDapper.Extensions
{
    internal static class StringExtension
    {
        /// <summary>
        /// Convert string to Snake Case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        internal static string ToPascalCase(this string input)
        {
            var yourString = input.ToLower().Replace("_", " ");
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            return info.ToTitleCase(yourString).Replace(" ", string.Empty);
        }

        internal static string ToCamelCase(this string input)
        {
            if (!string.IsNullOrEmpty(input) && input.Length > 1)
            {
                return char.ToLowerInvariant(input[0]) + input.Substring(1);
            }
            return input;
        }

        internal static string FormatByConvetion(this string input)
        {
            return FastManager.NameConvetion switch
            {
                NamingConvetion.PascalCase => input.ToPascalCase(),
                NamingConvetion.SnakeCase => input.ToSnakeCase(),
                NamingConvetion.CamelCase => input.ToCamelCase(),
                _ => input,
            };
        }
    }
}
