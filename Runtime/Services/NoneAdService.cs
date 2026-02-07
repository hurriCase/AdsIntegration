using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using PrimeTween;
using R3;
using UnityEngine;

namespace AdsIntegration.Runtime.Services
{
    [PublicAPI]
    public sealed class NoneAdService : IAdService
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAdAvailabilityChanged;
        private readonly ReactiveProperty<bool> _isRewardedAdAvailabilityChanged = new(true);

        private const float FakeAdsFinishDuration = 1f;

        public void Init() { }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
        {
            onRewarded += () => Debug.Log($"[NoneAdService::ShowRewardedAd] Rewarded ads was shown for {placement}");

            Tween.Delay(FakeAdsFinishDuration, onRewarded, useUnscaledTime: true);
            return true;
        }

        public bool TryShowInterstitial()
        {
            Debug.LogWarning("[NoneAdService::TryShowInterstitial] Interstitial ads was shown");
            return true;
        }
    }
}