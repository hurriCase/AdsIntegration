using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime.Services
{
    [PublicAPI]
    internal sealed class NoneAdService : IAdService
    {
        public Observable<bool> OnRewardedAdAvailabilityChanged => _rewardedAdAvailabilityChanged;
        private readonly Subject<bool> _rewardedAdAvailabilityChanged = new();

        public void Init() { }
        public bool ShowRewardedAd(Enum placement, Action onRewarded) => false;
        public bool IsRewardedAdAvailable() => false;
        public bool TryShowInterstitial() => false;
    }
}