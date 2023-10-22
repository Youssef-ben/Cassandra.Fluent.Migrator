namespace Cassandra.Fluent.Migrator.Utils.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Constants;
    using Enums;
    using Exceptions;
    using Microsoft.Rest.ClientRuntime.Azure.Authentication.Utilities;

    internal static class CSharpToCqlTypesExtensions
    {
        /// <summary>
        ///     Gets the equivalent CQL type of the supplied CSharp type.
        /// </summary>
        /// <param name="self">The CSharp Type.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the specified type is invalid or null.</exception>
        internal static string GetCqlType([NotNull] this Type self, bool shouldBeFrozen)
        {
            Check.NotNull(self, "The argument [type]");

            return self
                    .ConvertToCqlType(shouldBeFrozen)
                    .NormalizeString();
        }

        /// <summary>
        ///     Gets the equivalent CQL type from the Specified type column.
        /// </summary>
        /// <param name="self">Cassandra Table/User-Defined type.</param>
        /// <param name="column">Column name.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the type or the column is invalid or null.</exception>
        internal static string GetCqlType([NotNull] this Type self, [NotNull] string column, bool shouldBeFrozen)
        {
            Check.NotNull(self, "The argument [type]");

            return GetCqlTypeFromColumn(self, column, shouldBeFrozen)
                    .NormalizeString();
        }

        /// <summary>
        ///     Gets the Column equivalent CQL type from the Specified type.
        /// </summary>
        /// <param name="type">Type Cassandra Table/User-Defined type from which we need to get the column type.</param>
        /// <param name="column">The Column we should use to get the type.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>CQL Type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the type or the Column is invalid or null.</exception>
        /// <exception cref="ObjectNotFoundException">Thrown when the Column is not found in the specified object.</exception>
        private static string GetCqlTypeFromColumn([NotNull] Type type, [NotNull] string column, bool shouldBeFrozen)
        {
            Check.NotNull(type, $"The argument [{nameof(type)}]");
            Check.NotEmptyNotNull(column, $"The argument [{nameof(column)}]");

            Type instance = type.GetProperties()
                    .Select(prop => prop)
                    .FirstOrDefault(prop => prop.Name.Equals(column, StringComparison.OrdinalIgnoreCase))
                    ?.PropertyType;

            if (instance is null)
            {
                throw new ObjectNotFoundException(
                        AppErrorsMessages.NULL_REFERENCE.NormalizeString(column, type.Name.NormalizeString()));
            }

            return instance.ConvertToCqlType(shouldBeFrozen);
        }

        /// <summary>
        ///     Convert the CSharp/User-defined types to the equivalent Cassandra CQL type.
        /// </summary>
        /// <remarks>
        ///     It's a recursive method. It first tries to convert CSharp System types to CQL.<br />
        ///     If it doesn't get any results, it calls itself and tries with the next action type <br />
        ///     which are the system lists and user-defined lists. If still no success, <br />
        ///     it takes the last action to resolve the user-defined types. <br />
        ///     Finally, if no result obtained after these three attempts the method returns an error.
        /// </remarks>
        /// <param name="self">CSharp Type.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <param name="tryAction">The Try Action that should be executed to try and convert the CSharp type.</param>
        /// <returns>CQL Type.</returns>
        /// <exception cref="NotSupportedException">Thrown when the type is not supported or found.</exception>
        private static string ConvertToCqlType([NotNull] this Type self, bool shouldBeFrozen,
                TryConversionAction tryAction = TryConversionAction.SystemTypes)
        {
            Check.NotNull(self, "The argument [type]");

            var value = tryAction switch
            {
                TryConversionAction.SystemTypes => self.TryConvertToSystem(),
                TryConversionAction.ListTypes => self.TryConvertToList(shouldBeFrozen),
                TryConversionAction.UserDefinedTypes => self.TryConvertToUserDefinedType(shouldBeFrozen),
                _ => throw new NotSupportedException(AppErrorsMessages.NOT_SUPPORTED_TYPE.NormalizeString(self.Name))
            };

            if (!string.IsNullOrWhiteSpace(value) &&
                !ColumnTypeCode.List.NormalizeString().Equals(value.NormalizeString()))
            {
                return value;
            }

            // (Recursive call) If the value is empty try the next converting action.
            tryAction++;
            value = self.ConvertToCqlType(shouldBeFrozen, tryAction);

            return value;
        }

        /// <summary>
        ///     Convert the CSharp type to the Cassandra system CQL type.
        /// </summary>
        /// <param name="self">The Type object calling the method.</param>
        /// <returns>The CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the type is invalid or null.</exception>
        private static string TryConvertToSystem([NotNull] this Type self)
        {
            Check.NotNull(self, "The argument [type]");

            var name = self.Name.NormalizeString().Replace("`1", string.Empty);
            CSharpToCqlTypes.TypesMapping.TryGetValue(name, out var value);

            return value;
        }

        /// <summary>
        ///     Convert the CSharp Lists to the cassandra CQL Lists.
        /// </summary>
        /// <param name="self">The Type object calling the method.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the type is invalid or null.</exception>
        private static string TryConvertToList([NotNull] this Type self, bool shouldBeFrozen)
        {
            Check.NotNull(self, "The argument [type]");

            switch (self.Name.NormalizeString().Replace("`1", string.Empty))
            {
                case "list":
                case "ilist":
                case "hashset":
                case "ienumerable":
                case "icollection":
                    Type genericType = self.GetGenericArguments().FirstOrDefault();
                    if (genericType is null)
                    {
                        return string.Empty;
                    }

                    if (genericType.Namespace != null && genericType.Namespace.StartsWith("System"))
                    {
                        return $"list<{genericType.ConvertToCqlType(shouldBeFrozen)}>";
                    }

                    if (genericType.BaseType != null && genericType.BaseType.Name.StartsWith("Object"))
                    {
                        return $"frozen<list<frozen<{genericType.Name.ToLower()}>>>";
                    }

                    break;

                case "idictionary":
                    Type[] genericTypes = self.GetGenericArguments();
                    if (genericTypes is null || genericTypes.Length <= 1)
                    {
                        return string.Empty;
                    }

                    return $"Map<{genericTypes[0].ConvertToCqlType(true)},{genericTypes[1].ConvertToCqlType(true)}>";
            }

            return string.Empty;
        }

        /// <summary>
        ///     Convert the CSharp class to the Cassandra User-Defined Type.
        /// </summary>
        /// <param name="self">The Type object calling the method.</param>
        /// <param name="shouldBeFrozen">Define if the type should be treated as a frozen type or not.</param>
        /// <returns>The CQL type.</returns>
        /// <exception cref="NullReferenceException">Thrown when the type is invalid or null.</exception>
        private static string TryConvertToUserDefinedType([NotNull] this Type self, bool shouldBeFrozen)
        {
            Check.NotNull(self, "The argument [type]");

            var genericType = self.Name.NormalizeString();

            if (string.IsNullOrWhiteSpace(genericType))
            {
                return string.Empty;
            }

            return shouldBeFrozen ? $"frozen<{genericType}>" : genericType;
        }
    }
}