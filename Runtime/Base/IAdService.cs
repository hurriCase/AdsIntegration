using System;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime.Base
{
    /// <summary>
    /// Primary interface for interacting with the ad service. Provides methods for initializing,
    /// showing ads, and checking ad availability.
    /// </summary>
    [PublicAPI]
    public interface IAdService
    {
        Observable<bool> OnRewardedAdAvailabilityChanged { get; }

        void Init();
        bool ShowRewardedAd(Enum placement, Action onRewarded);
        bool IsRewardedAdAvailable();
        bool TryShowInterstitial();
    }
}