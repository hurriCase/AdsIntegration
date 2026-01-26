using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace AdsIntegration.Runtime.Services.Azerion
{
    [Resource(FullSettingsPath, nameof(AzerionConfig), ResourceSettingsPath)]
    internal sealed class AzerionConfig : SingletonScriptableObject<AzerionConfig>
    {
        [field: SerializeField] internal GameDistribution GameDistributionPrefab { get; private set; }
        [field: SerializeField] internal string GameKey { get; private set; }

        private const string FullSettingsPath = "Assets/Resources/" + ResourceSettingsPath;
        private const string ResourceSettingsPath = "Azerion";
    }
}