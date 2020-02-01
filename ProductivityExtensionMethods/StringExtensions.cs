#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace System
{
    [GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class StringExtensions
    {
        #region Public Methods

        #if SUPPORT_NETSTANDARD2_1_AND_ABOVE
        /// <summary>
        /// Executes the String.IsNullOrWhiteSpace on the current string
        /// </summary>
        /// <returns>True if either empty, white space or null</returns>
        public static bool IsBlank([NotNullWhen(false)] this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        [return: NotNullIfNotNull("defaultValue")]
        public static string? ValueOrDefault(this string? value, string? defaultValue)
        {
            return value.IsBlank() ? defaultValue : value;
        }

        #else

        /// <summary>
        ///     Executes the String.IsNullOrWhiteSpace on the current string
        /// </summary>
        /// <returns>True if either empty, white space or null</returns>
        public static bool IsBlank(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string? ValueOrDefault(this string? value, string? defaultValue)
        {
            return value.IsBlank() ? defaultValue : value;
        }

        #endif

        #if (CORE2_1_AND_ABOVE || SUPPORT_NETSTANDARD2_1_AND_ABOVE)
        public static ReadOnlySpan<char> SubstringAfter(this ReadOnlySpan<char> input, ReadOnlySpan<char> value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;

            return input.Slice(i + value.Length);
        }

        public static ReadOnlySpan<char> SubstringAfter(this ReadOnlySpan<char> input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;

            return input.Slice(i + 1);
        }

        #endif

        public static string SubstringAfter(this string input, string value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;

            return input.Substring(i + value.Length);
        }

        #if !SUPPORT_NETSTANDARD2_1_AND_ABOVE
        public static string SubstringAfter(this string input, char value, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            int i = input.IndexOf(value, stringComparison);
            if (i == -1)
                return string.Empty;

            return input.Substring(i + 1);
        }

        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, char separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }

        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, string separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }

        public static StringBuilder StringBuilderJoin(this string[] values, char separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }

        public static StringBuilder StringBuilderJoin(this string[] values, string separator)
        {
            return new StringBuilder().AppendJoin(separator, values);
        }

        #else

        public static string SubstringAfter(this string input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;

            return input.Substring(i + 1);
        }

        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, char separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - 1, 1);

            return result;
        }

        public static StringBuilder StringBuilderJoin(this string[] values, char separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - 1, 1);

            return result;
        }

        public static StringBuilder StringBuilderJoin(this IEnumerable<string> values, string separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result;
        }

        public static StringBuilder StringBuilderJoin(this string[] values, string separator)
        {
            var result = new StringBuilder();
            foreach (string st in values)
                result.Append(st).Append(separator);

            if (result.Length > 0)
                result.Remove(result.Length - separator.Length, separator.Length);

            return result;
        }
        #endif

        public static string SubstringBefore(this string input, string value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;

            return input.Substring(0, i);
        }

        public static string SubstringBefore(this string input, char value)
        {
            int i = input.IndexOf(value);
            if (i == -1)
                return string.Empty;

            return input.Substring(0, i);
        }

        #if (CORE2_1_AND_ABOVE || SUPPORT_NETSTANDARD2_1_AND_ABOVE)
        /// <summary>
        /// Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static ReadOnlySpan<char> LimitLength(this ReadOnlySpan<char> source, int maxLength)
        {
            if (source.Length <= maxLength)
                return source;

            return source.Slice(0, maxLength);
        }

        #endif
        /// <summary>
        ///     Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        /// <param name="addEllipse">whether or not to add an ellipse to show string is truncated</param>
        public static string LimitLength(this string source, int maxLength, bool addEllipse)
        {
            if (source.Length <= maxLength)
                return source;

            if (maxLength <= 4)
                addEllipse = false;
            if (addEllipse)
                maxLength -= 3;

            string result = source.Substring(0, maxLength);

            if (addEllipse)
                result += "...";

            return result;
        }

        /// <summary>
        ///     Method that limits the length of text to a defined length.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="maxLength">The maximum limit of the string to return.</param>
        public static string LimitLength(this string source, int maxLength)
        {
            return LimitLength(source, maxLength, false);
        }

        public static string StringJoin(this string[] values, string separator)
        {
            return string.Join(separator, values);
        }

        public static string StringJoin(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static string StringJoin(this string[] values, char separator)
        {
            return string.Join(separator, values);
        }

        public static string StringJoin(this IEnumerable<string> values, char separator)
        {
            return string.Join(separator, values);
        }

        #endregion
    }
}