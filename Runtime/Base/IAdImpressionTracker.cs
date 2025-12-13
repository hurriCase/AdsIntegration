using System;
using JetBrains.Annotations;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Interface for tracking ad impressions from ad networks.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface should handle reporting ad impression data
    /// to analytics services or other tracking systems.
    /// </remarks>
    [UsedImplicitly]
    public interface IAdImpressionTracker : IDisposable
    {
        /// <summary>
        /// Tracks an ad impression from IronSource.
        /// </summary>
        /// <param name="impressionData">The impression data provided by IronSource SDK.</param>
        /// <remarks>
        /// This method should be called whenever an ad impression event is received from IronSource.
        /// It handles processing and forwarding the impression data to appropriate analytics services.
        /// </remarks>
        [UsedImplicitly]
        void TrackAdImpression(LevelPlayImpressionData impressionData);
    }
}