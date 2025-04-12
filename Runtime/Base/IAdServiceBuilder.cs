using System;
using AdsIntegration.Runtime.Config;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Builder interface for creating ad service instances
    /// </summary>
    public interface IAdServiceBuilder
    {
        /// <summary>
        /// Enable debug logging
        /// </summary>
        IAdServiceBuilder WithDebugLogging();

        /// <summary>
        /// Enable test mode for ad networks
        /// </summary>
        IAdServiceBuilder WithTestMode();

        /// <summary>
        /// Set the callback for when initialization succeeds
        /// </summary>
        IAdServiceBuilder OnInitialized(Action callback);

        /// <summary>
        /// Set the callback for when initialization fails
        /// </summary>
        IAdServiceBuilder OnInitializationFailed(Action<string> callback);

        /// <summary>
        /// Set a callback for when rewarded ad availability changes
        /// </summary>
        IAdServiceBuilder OnRewardedAdAvailabilityChanged(Action<bool> callback);

        /// <summary>
        /// Build and return the ad service instance
        /// </summary>
        IAdService Build();
    }
}