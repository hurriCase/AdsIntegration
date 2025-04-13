using System;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Builder interface for creating ad service instances.
    /// </summary>
    /// <remarks>
    /// Use this interface to configure and build ad service instances with the desired settings and callbacks.
    /// </remarks>
    public interface IAdServiceBuilder
    {
        /// <summary>
        /// Enables debug logging for the ad service.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// When enabled, the ad service will log detailed information about its operations to the Unity console.
        /// This is useful for debugging integration issues during development.
        /// </remarks>
        IAdServiceBuilder WithDebugLogging();

        /// <summary>
        /// Enables test mode for the ad service.
        /// </summary>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// In test mode, the ad service will use test ad units and may show special UI for testing purposes.
        /// This should only be used during development and testing, not in production builds.
        /// </remarks>
        IAdServiceBuilder WithTestMode();

        /// <summary>
        /// Configures the ad service to use the specified impression tracker for analytics.
        /// </summary>
        /// <param name="adImpressionTracker">The implementation of IAdImpressionTracker to use.</param>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// The impression tracker will receive callbacks when ads are displayed, allowing revenue
        /// and other metrics to be tracked in analytics services.
        /// </remarks>
        IAdServiceBuilder WithAnalyticsService(IAdImpressionTracker adImpressionTracker);

        /// <summary>
        /// Sets a callback to be invoked when the ad service is successfully initialized.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>The builder instance for method chaining.</returns>
        IAdServiceBuilder OnInitialized(Action callback);

        /// <summary>
        /// Sets a callback to be invoked when ad service initialization fails.
        /// </summary>
        /// <param name="callback">The callback to invoke with the error message.</param>
        /// <returns>The builder instance for method chaining.</returns>
        IAdServiceBuilder OnInitializationFailed(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when rewarded ad availability changes.
        /// </summary>
        /// <param name="callback">The callback to invoke with the availability status.</param>
        /// <returns>The builder instance for method chaining.</returns>
        /// <remarks>
        /// The boolean parameter indicates whether rewarded ads are available (true) or not (false).
        /// </remarks>
        IAdServiceBuilder OnRewardedAdAvailabilityChanged(Action<bool> callback);

        /// <summary>
        /// Sets a callback to be invoked when a rewarded ad begins playing.
        /// </summary>
        /// <param name="callback">The callback to invoke with the placement name.</param>
        /// <returns>The builder instance for method chaining.</returns>
        IAdServiceBuilder OnRewardedAdShowStarted(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when a rewarded ad completes and the reward should be granted.
        /// </summary>
        /// <param name="callback">The callback to invoke with the placement name.</param>
        /// <returns>The builder instance for method chaining.</returns>
        IAdServiceBuilder OnRewardedAdRewarded(Action<string> callback);

        /// <summary>
        /// Sets a callback to be invoked when an interstitial ad begins playing.
        /// </summary>
        /// <param name="callback">The callback to invoke with the ad unit ID.</param>
        /// <returns>The builder instance for method chaining.</returns>
        IAdServiceBuilder OnInterstitialAdShowStarted(Action<string> callback);

        /// <summary>
        /// Builds and returns an ad service instance with the configured settings and callbacks.
        /// </summary>
        /// <returns>A fully initialized IAdService instance.</returns>
        /// <remarks>
        /// The returned service is not yet initialized. You must call Initialize() on the returned instance
        /// to begin the initialization process.
        /// </remarks>
        IAdService Build();
    }
}