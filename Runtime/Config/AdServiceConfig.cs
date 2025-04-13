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
    internal sealed class AdServiceConfig : ScriptableObject
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

        internal void SetPlacementEnumType(Type enumType)
        {
            if (enumType.IsEnum is false)
            {
                Debug.LogError($"Type {enumType.Name} is not an enum.");
                return;
            }

            PlacementEnumType = enumType.AssemblyQualifiedName;
            PlacementDefinitions = PlacementExtensions.GetPlacementDefinitions(enumType);
        }

        internal Type GetPlacementEnumType()
            => string.IsNullOrEmpty(PlacementEnumType) ? null : Type.GetType(PlacementEnumType);

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