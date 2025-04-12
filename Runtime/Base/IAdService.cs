using System;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Main interface for interacting with the ad service
    /// </summary>
    public interface IAdService
    {
        /// <summary>
        /// Event triggered when ad availability status changes
        /// </summary>
        event Action<bool> OnRewardedAdAvailabilityChanged;

        /// <summary>
        /// Event triggered when the service is successfully initialized
        /// </summary>
        event Action OnInitialized;

        /// <summary>
        /// Event triggered when initialization fails
        /// </summary>
        event Action<string> OnInitializationFailed;

        /// <summary>
        /// Initialize the ad service
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shows a rewarded ad for the specified placement
        /// </summary>
        /// <param name="placementName">The name of the placement</param>
        /// <param name="onRewarded">Callback to execute when the user earns the reward</param>
        /// <returns>True if the ad began showing, false otherwise</returns>
        bool ShowRewardedAd(string placementName, Action onRewarded);

        /// <summary>
        /// Shows a rewarded ad for the specified placement enum value
        /// </summary>
        /// <param name="placement">The placement enum value</param>
        /// <param name="onRewarded">Callback to execute when the user earns the reward</param>
        /// <returns>True if the ad began showing, false otherwise</returns>
        bool ShowRewardedAd(Enum placement, Action onRewarded);

        /// <summary>
        /// Checks if a rewarded ad is available to show
        /// </summary>
        bool IsRewardedAdAvailable();

        /// <summary>
        /// Tries to show an interstitial ad if one is available and conditions are met
        /// </summary>
        /// <returns>True if an interstitial was shown, false otherwise</returns>
        bool TryShowInterstitial();

        /// <summary>
        /// Enable test mode for ad networks
        /// </summary>
        void EnableTestMode();
    }
}