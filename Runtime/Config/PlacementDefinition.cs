using System;
using UnityEngine;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Defines a placement and its associated reward type
    /// </summary>
    [Serializable]
    public class PlacementDefinition
    {
        [field: SerializeField] public string PlacementName { get; private set; }
        [field: SerializeField] public string RewardType { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        /// <summary>
        /// Creates a new placement definition
        /// </summary>
        public PlacementDefinition(string placementName, string rewardType, string description = "")
        {
            PlacementName = placementName;
            RewardType = rewardType;
            Description = description;
        }

        // Default constructor for serialization
        public PlacementDefinition() { }
    }
}