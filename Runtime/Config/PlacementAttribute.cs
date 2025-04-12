using System;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Attribute to mark enum values as ad placements and associate them with reward types
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PlacementAttribute : Attribute
    {
        /// <summary>
        /// The reward type associated with this placement
        /// </summary>
        public string RewardType { get; }

        /// <summary>
        /// Optional description of this placement
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Create a new placement attribute with the specified reward type
        /// </summary>
        /// <param name="rewardType">The reward type for this placement</param>
        /// <param name="description">Optional description of this placement</param>
        public PlacementAttribute(string rewardType, string description = "")
        {
            RewardType = rewardType;
            Description = description;
        }
    }
}