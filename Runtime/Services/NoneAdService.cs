using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime.Services
{
    [PublicAPI]
    public sealed class NoneAdService : IAdService
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAdAvailabilityChanged;
        private readonly ReactiveProperty<bool> _isRewardedAdAvailabilityChanged = new();

        public void Init() { }
        public bool ShowRewardedAd(Enum placement, Action onRewarded) => false;
        public bool TryShowInterstitial() => false;
    }
}