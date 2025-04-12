using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Scriptable object to store advertising service configuration data
    /// </summary>
    public sealed class AdServiceConfig : ScriptableObject
    {
        [field: Header("IronSource Settings")]
        [field: SerializeField] internal string AppKey { get; private set; }
        [field: SerializeField] internal string RewardedAdUnitId { get; private set; }
        [field: SerializeField] internal string InterstitialAdUnitId { get; private set; }

        [field: Header("Ad Settings")]
        [field: SerializeField] internal float TimeBetweenInterstitials { get; private set; } = 60f;
        [field: SerializeField] internal int MaxInterstitialLoadAttempts { get; private set; } = 3;
        [field: SerializeField] internal int MaxRewardedLoadAttempts { get; private set; } = 3;
        [field: SerializeField] internal float RetryLoadDelay { get; private set; } = 30f;

        [field: Header("Placement Configuration")]
        [field: SerializeField] internal List<PlacementDefinition> PlacementDefinitions { get; private set; } = new();

        [field: Header("Enum Placement Type")]
        [field: SerializeField] internal string PlacementEnumType { get; private set; }

        private const string SettingsPath = "Assets/AdsIntegration/Resources/AdServiceConfig.asset";

        /// <summary>
        /// Sets the placement enum type and loads all placement definitions from it
        /// </summary>
        public void SetPlacementEnumType(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                Debug.LogError($"Type {enumType.Name} is not an enum.");
                return;
            }

            PlacementEnumType = enumType.AssemblyQualifiedName;
            PlacementDefinitions = PlacementExtensions.GetPlacementDefinitions(enumType);
        }

        /// <summary>
        /// Gets the placement enum type if one is set
        /// </summary>
        public Type GetPlacementEnumType()
        {
            if (string.IsNullOrEmpty(PlacementEnumType))
                return null;

            return Type.GetType(PlacementEnumType);
        }

        /// <summary>
        /// Finds the reward type for a placement
        /// </summary>
        internal string FindRewardTypeForPlacement(string placementName)
        {
            foreach (var placement in PlacementDefinitions)
            {
                if (placement.PlacementName == placementName)
                    return placement.RewardType;
            }

            Debug.LogWarning($"No reward type found for placement: {placementName}, using placement name as reward type");
            return placementName;
        }

        internal static AdServiceConfig GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AdServiceConfig>(SettingsPath);
            if (settings)
                return settings;

            settings = CreateInstance<AdServiceConfig>();

            var directory = Path.GetDirectoryName(SettingsPath);
            if (string.IsNullOrEmpty(directory) is false && Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();

            return settings;
        }

        internal static bool SettingsExist() => AssetDatabase.LoadAssetAtPath<AdServiceConfig>(SettingsPath);
    }
}