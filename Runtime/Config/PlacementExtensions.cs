using System;
using System.Collections.Generic;
using System.Reflection;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Extension methods for working with placement enums
    /// </summary>
    internal static class PlacementExtensions
    {
        private static readonly Dictionary<Type, bool> _placementEnumCache = new();

        /// <summary>
        /// Gets all placement definitions from an enum type.
        /// </summary>
        /// <param name="enumType">The enum type to scan for placements.</param>
        /// <returns>A list of PlacementDefinition objects.</returns>
        /// <exception cref="ArgumentException">Thrown if the type is not an enum or not marked with PlacementAttribute.</exception>
        internal static List<PlacementDefinition> GetPlacementDefinitions(Type enumType)
        {
            if (enumType.IsEnum is false)
                throw new ArgumentException("[PlacementExtensions::GetPlacementDefinitions] " +
                                            $"Type {enumType.Name} is not an enum.");

            if (IsPlacementEnum(enumType) is false)
                throw new ArgumentException("[PlacementExtensions::GetPlacementDefinitions] " +
                                            $"Type {enumType.Name} is not marked with the PlacementAttribute.");

            var result = new List<PlacementDefinition>();

            foreach (Enum value in Enum.GetValues(enumType))
            {
                var placementName = value.ToString();
                result.Add(new PlacementDefinition(placementName));
            }

            return result;
        }

        /// <summary>
        /// Gets the placement name for an enum value.
        /// </summary>
        /// <param name="placement">The enum value representing a placement.</param>
        /// <returns>The string representation of the enum value to use as the placement name.</returns>
        internal static string GetPlacementName(this Enum placement) => placement.ToString();

        /// <summary>
        /// Checks if an enum type is marked with the PlacementAttribute.
        /// </summary>
        /// <param name="enumType">The enum type to check.</param>
        /// <returns>True if the enum type is marked with PlacementAttribute, otherwise false.</returns>
        private static bool IsPlacementEnum(Type enumType)
        {
            if (_placementEnumCache.TryGetValue(enumType, out var isPlacementEnum))
                return isPlacementEnum;

            isPlacementEnum = enumType.GetCustomAttribute<PlacementAttribute>() != null;
            _placementEnumCache[enumType] = isPlacementEnum;

            return isPlacementEnum;
        }
    }
}