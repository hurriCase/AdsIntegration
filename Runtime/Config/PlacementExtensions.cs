using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Extension methods for working with placement enums
    /// </summary>
    public static class PlacementExtensions
    {
        private static readonly Dictionary<Type, Dictionary<Enum, PlacementAttribute>> AttributeCache = new();

        /// <summary>
        /// Gets the placement name for an enum value (which is the enum value's name)
        /// </summary>
        public static string GetPlacementName(this Enum placement)
        {
            return placement.ToString();
        }

        /// <summary>
        /// Gets the reward type associated with a placement enum value
        /// </summary>
        public static string GetRewardType(this Enum placement)
        {
            var attribute = GetPlacementAttribute(placement);
            return attribute?.RewardType ?? placement.ToString();
        }

        /// <summary>
        /// Gets the description associated with a placement enum value
        /// </summary>
        public static string GetDescription(this Enum placement)
        {
            var attribute = GetPlacementAttribute(placement);
            return attribute?.Description ?? string.Empty;
        }

        /// <summary>
        /// Gets all placement definitions from an enum type
        /// </summary>
        public static List<PlacementDefinition> GetPlacementDefinitions(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Type {enumType.Name} is not an enum.");
            }

            var result = new List<PlacementDefinition>();

            foreach (Enum value in Enum.GetValues(enumType))
            {
                var placementName = value.ToString();
                var attribute = GetPlacementAttribute(value);

                if (attribute != null)
                {
                    result.Add(new PlacementDefinition(
                        placementName,
                        attribute.RewardType,
                        attribute.Description
                    ));
                }
            }

            return result;
        }

        private static PlacementAttribute GetPlacementAttribute(Enum enumValue)
        {
            var enumType = enumValue.GetType();

            // Check if we've already cached this enum type
            if (!AttributeCache.TryGetValue(enumType, out var attributeMap))
            {
                attributeMap = new Dictionary<Enum, PlacementAttribute>();
                AttributeCache[enumType] = attributeMap;

                // Cache all values for this enum type
                foreach (Enum value in Enum.GetValues(enumType))
                {
                    var field = enumType.GetField(value.ToString());
                    var attribute = field.GetCustomAttribute<PlacementAttribute>();
                    if (attribute != null)
                    {
                        attributeMap[value] = attribute;
                    }
                }
            }

            return attributeMap.TryGetValue(enumValue, out var result) ? result : null;
        }
    }
}