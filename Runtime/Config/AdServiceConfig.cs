using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AdsIntegration.Runtime.Config
{
    /// <inheritdoc />
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

        internal static bool IsSettingsExist => Resources.Load<AdServiceConfig>(ResourceSettingsPath);

        private const string FullSettingsPath = "Assets/Resources/" + ResourceSettingsPath;
        private const string ResourceSettingsPath = "AdsIntegration/AdServiceConfig.asset";

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
            var settings = Resources.Load<AdServiceConfig>($"{nameof(AdServiceConfig)}.asset");
            if (settings)
                return settings;

#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<AdServiceConfig>(FullSettingsPath);
            if (settings)
                return settings;

            settings = CreateInstance<AdServiceConfig>();

            var directory = Path.GetDirectoryName(FullSettingsPath);
            if (string.IsNullOrEmpty(directory) is false && Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            AssetDatabase.CreateAsset(settings, FullSettingsPath);
            AssetDatabase.SaveAssets();

            return settings;
#endif
        }
    }
}