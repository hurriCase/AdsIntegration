using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace AdsIntegration.Runtime.Services.Azerion
{
    [Resource(ResourcePath, nameof(AzerionConfiguration), ResourcePath)]
    internal sealed class AzerionConfiguration : SingletonScriptableObject<AzerionConfiguration>
    {
        [field: SerializeField] internal GameDistribution GameDistributionPrefab { get; private set; }
        [field: SerializeField] internal string GameKey { get; private set; }

        private const string ResourcePath = "Assets/Resources/Azerion/";
    }
}