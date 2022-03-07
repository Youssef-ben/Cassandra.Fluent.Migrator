namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class CommonHelpersExtensions
    {
        private static CultureInfo DEFAULT_CULTURE => new CultureInfo("en-US", false);

        /// <summary>
        ///  Returns a new string whose textual value is the same as this string,
        ///  but whose binary representation is in Unicode normalization form C
        ///  then return a lowercase form.
        /// </summary>
        ///
        /// <param name="self">The string to be formatted.</param>
        /// <returns>Normalized string.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are null or empty.</exception>
        internal static string NormalizeString([NotNull]this ColumnTypeCode self)
        {
            Check.NotNull(self, $"The argument [Column type code]");

            return self.ToString().Normalize().ToLower(DEFAULT_CULTURE);
        }

        /// <summary>
        ///  Returns a new string whose textual value is the same as this string,
        ///  but whose binary representation is in Unicode normalization form C
        ///  then return a lowercase form.
        /// </summary>
        ///
        /// <param name="self">The string to be formatted.</param>
        /// <returns>Normalized string.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are null or empty.</exception>
        internal static string NormalizeString([NotNull]this string self)
        {
            Check.NotEmptyNotNull(self, $"The method caller");

            return self.Trim().Normalize().ToLower(DEFAULT_CULTURE);
        }

        /// <summary>
        /// Format and normalize the given string based on the arguments passed to it.
        /// </summary>
        ///
        /// <param name="self">The string object calling this method.</param>
        /// <param name="args">Values to use for formatting the string.</param>
        /// <returns>Returns normalized and formatted text.</returns>
        ///
        /// <exception cref="NullReferenceException">Thrown when the arguments are null or empty.</exception>
        internal static string NormalizeString([NotNull]this string self, params string[] args)
        {
            Check.NotEmptyNotNull(self, $"The method caller");

            return string.Format(self, args).NormalizeString();
        }
    }
}
