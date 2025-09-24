using System;
using JetBrains.Annotations;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Builder interface for creating ad service instances.
    /// </summary>
    /// <remarks>
    /// Use this interface to configure and build ad service instances with the desired settings and callbacks.
    /// </remarks>
    [UsedImplicitly]
    public interface IAdServiceBuilder
    {
        /// <summary>
        /// Configures the ad service to use the specified impression tracker for analytics.
        /// </summary>
        /// <param name="adImpressionTracker">The implementation of IAdImpressionTracker to use.</param>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// The impression tracker will receive callbacks when ads are displayed, allowing revenue
        /// and other metrics to be tracked in analytics services.
        /// </remarks>
        [UsedImplicitly]
        IAdServiceBuilder WithAnalyticsService(IAdImpressionTracker adImpressionTracker);

        /// <summary>
        /// Sets a callback to be invoked when the ad service is successfully initialized.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [UsedImplicitly]
        IAdServiceBuilder OnInitialized(Action callback);

        /// <summary>
        /// Sets a callback to be invoked when ad service initialization fails.
        /// </summary>
        /// <param name="callback">The callback to invoke with the error message.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [UsedImplicitly]
        IAdServiceBuilder OnInitializationFailed(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when rewarded ad availability changes.
        /// </summary>
        /// <param name="callback">The callback to invoke with the availability status.</param>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// The boolean parameter indicates whether rewarded ads are available (true) or not (false).
        /// </remarks>
        [UsedImplicitly]
        IAdServiceBuilder OnRewardedAdAvailabilityChanged(Action<bool> callback);

        /// <summary>
        /// Sets a callback to be invoked when a rewarded ad begins playing.
        /// </summary>
        /// <param name="callback">The callback to invoke with the placement name.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [UsedImplicitly]
        IAdServiceBuilder OnRewardedAdShowStarted(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when a rewarded ad completes and the reward should be granted.
        /// </summary>
        /// <param name="callback">The callback to invoke with the placement name.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [UsedImplicitly]
        IAdServiceBuilder OnRewardedAdRewarded(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when an interstitial ad begins playing.
        /// </summary>
        /// <param name="callback">The callback to invoke with the ad unit ID.</param>
        /// <returns>The builder instance for method chaining.</returns>
        [UsedImplicitly]
        IAdServiceBuilder OnInterstitialAdShowStarted(Action<string> callback);

        /// <summary>
        /// Builds and returns an ad service instance with the configured settings and callbacks.
        /// </summary>
        /// <returns>A fully initialized IAdService instance.</returns>
        /// <remarks>
        /// The returned service is not yet initialized. You must call Initialize() on the returned instance
        /// to begin the initialization process.
        /// </remarks>
        [UsedImplicitly]
        IAdService Build();
    }
}