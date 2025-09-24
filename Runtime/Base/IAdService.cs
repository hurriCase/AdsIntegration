using System;
using JetBrains.Annotations;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Primary interface for interacting with the ad service. Provides methods for initializing,
    /// showing ads, and checking ad availability.
    /// </summary>
    [UsedImplicitly]
    public interface IAdService
    {
        /// <summary>
        /// Event that fires when the ad service has been successfully initialized.
        /// </summary>
        [UsedImplicitly]
        event Action OnInitialized;

        /// <summary>
        /// Event that fires when the ad service initialization fails.
        /// </summary>
        /// <remarks>The string parameter contains the error message.</remarks>
        [UsedImplicitly]
        event Action<string> OnInitializationFailed;

        /// <summary>
        /// Event that fires when the availability of rewarded ads changes.
        /// </summary>
        /// <remarks>The boolean parameter indicates whether rewarded ads are available (true) or not (false).</remarks>
        [UsedImplicitly]
        event Action<bool> OnRewardedAdAvailabilityChanged;

        /// <summary>
        /// Event that fires when a rewarded ad begins playing.
        /// </summary>
        /// <remarks>The string parameter contains the placement name.</remarks>
        [UsedImplicitly]
        event Action<string> OnRewardedAdShowStarted;

        /// <summary>
        /// Event that fires when a rewarded ad completes and the reward should be granted.
        /// </summary>
        /// <remarks>The string parameter contains the placement name.</remarks>
        [UsedImplicitly]
        event Action<string> OnRewardedAdRewarded;

        /// <summary>
        /// Event that fires when an interstitial ad begins playing.
        /// </summary>
        /// <remarks>The string parameter contains the ad unit ID.</remarks>
        [UsedImplicitly]
        event Action<string> OnInterstitialAdShowStarted;

        /// <summary>
        /// Initializes the ad service and underlying SDK.
        /// </summary>
        /// <remarks>
        /// This should be called early in your application lifecycle, typically during app startup.
        /// The <see cref="OnInitialized"/> event will fire when initialization completes successfully.
        /// </remarks>
        [UsedImplicitly]
        void Init();

        /// <summary>
        /// Shows a rewarded ad for the specified placement enum.
        /// </summary>
        /// <param name="placement">The enum value that defines the ad placement.</param>
        /// <param name="onRewarded">Callback that will be invoked when the user earns the reward.</param>
        /// <returns>True if the ad was shown, false otherwise (e.g., no ad available).</returns>
        /// <remarks>
        /// The placement enum should be decorated with the PlacementAttribute/>.
        /// </remarks>
        [UsedImplicitly]
        bool ShowRewardedAd(Enum placement, Action onRewarded);

        /// <summary>
        /// Shows a rewarded ad for the specified placement name.
        /// </summary>
        /// <param name="placementName">The string name of the placement.</param>
        /// <param name="onRewarded">Callback that will be invoked when the user earns the reward.</param>
        /// <returns>True if the ad was shown, false otherwise (e.g., no ad available).</returns>
        [UsedImplicitly]
        bool ShowRewardedAd(string placementName, Action onRewarded);

        /// <summary>
        /// Checks if a rewarded ad is currently available to show.
        /// </summary>
        /// <returns>True if a rewarded ad is available, false otherwise.</returns>
        [UsedImplicitly]
        bool IsRewardedAdAvailable();

        /// <summary>
        /// Attempts to show an interstitial ad.
        /// </summary>
        /// <returns>True if the ad was shown, false otherwise.</returns>
        /// <remarks>
        /// Interstitial ads have cooldown periods between displays. This method will
        /// automatically respect those cooldowns.
        /// </remarks>
        [UsedImplicitly]
        bool TryShowInterstitial();
    }
}