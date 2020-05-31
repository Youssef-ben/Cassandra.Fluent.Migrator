namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class CommonHelpersExtensions
    {
        private static CultureInfo DEFAULT_CUTURE => new CultureInfo("en-US", false);

        /// <summary>
        ///  Returns a new string whose textual value is the same as this string,
        ///  but whose binary representation is in Unicode normalization form C
        ///  then return a lowercase form.
        /// </summary>
        /// <param name="self">The string to be formatted.</param>
        /// <returns>Normalized string.</returns>
        internal static string NormalizeString([NotNull]this ColumnTypeCode self)
        {
            Check.NotNull(self, $"The argument [Column type code]");

            return self.ToString().Normalize().ToLower(DEFAULT_CUTURE);
        }

        /// <summary>
        ///  Returns a new string whose textual value is the same as this string,
        ///  but whose binary representation is in Unicode normalization form C
        ///  then return a lowercase form.
        /// </summary>
        /// <param name="self">The string to be formatted.</param>
        /// <returns>Normalized string.</returns>
        internal static string NormalizeString([NotNull]this string self)
        {
            Check.NotEmptyNotNull(self, $"The method caller");

            return self.Trim().Normalize().ToLower(DEFAULT_CUTURE);
        }

        /// <summary>
        /// Format and returns a new string whose textual value is the same as this string,
        /// but whose binary representation is in Unicode normalization form C
        /// then return a lowercase form.
        /// Format and normalize the given string based on the arguments passed to it.
        /// </summary>
        /// <param name="self">The string object calling this method.</param>
        /// <param name="args">The target string values.</param>
        /// <returns>Returns normalized and formatted text.</returns>
        internal static string NormalizeString([NotNull]this string self, params string[] args)
        {
            Check.NotEmptyNotNull(self, $"The method caller");

            return string.Format(self, args).NormalizeString();
        }

        /// <summary>
        /// Validate that this string contains the targeted value.
        /// Checks if the string object calling this method contains the targeted value.
        /// </summary>
        /// <param name="self">The string object calling this method.</param>
        /// <param name="targetText">The target string to look up.</param>
        /// <param name="args">The target string arguments.</param>
        /// <returns>True if value exists. False otherwise.</returns>
        internal static bool ContainsText(this string self, string targetText, params string[] args)
        {
            return self.NormalizeString().Contains(targetText.NormalizeString(args));
        }
    }
}
