using System;
using UnityEngine;

namespace AdsIntegration.Runtime.Config
{
    [Serializable]
    internal class PlacementDefinition
    {
        [field: SerializeField] internal string PlacementName { get; private set; }

        internal PlacementDefinition(string placementName)
        {
            PlacementName = placementName;
        }

        internal PlacementDefinition() { }
    }
}