using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using UnityEngine;

namespace AdsIntegration.Runtime.Builder
{
    internal sealed class IronSourceAdServiceBuilder : IAdServiceBuilder
    {
        private IAdImpressionTracker _adImpressionTracker;
        private Action _onInitializedCallback;
        private Action<string> _onInitFailedCallback;
        private Action<bool> _onRewardedAdAvailabilityChangedCallback;

        private Action<string> _onRewardedAdShowStartedCallback;
        private Action<string> _onRewardedAdRewardedCallback;
        private Action<string> _onInterstitialAdShowStartedCallback;
        private Action<string> _onInterstitialAdShowEndedCallback;

        public IAdServiceBuilder WithAnalyticsService(IAdImpressionTracker adImpressionTracker)
        {
            _adImpressionTracker = adImpressionTracker;
            return this;
        }

        public IAdServiceBuilder OnInitialized(Action callback)
        {
            _onInitializedCallback = callback;
            return this;
        }

        public IAdServiceBuilder OnInitializationFailed(Action<string> callback)
        {
            _onInitFailedCallback = callback;
            return this;
        }

        public IAdServiceBuilder OnRewardedAdAvailabilityChanged(Action<bool> callback)
        {
            _onRewardedAdAvailabilityChangedCallback = callback;
            return this;
        }

        public IAdServiceBuilder OnRewardedAdShowStarted(Action<string> callback)
        {
            _onRewardedAdShowStartedCallback = callback;
            return this;
        }

        public IAdServiceBuilder OnRewardedAdRewarded(Action<string> callback)
        {
            _onRewardedAdRewardedCallback = callback;
            return this;
        }

        public IAdServiceBuilder OnInterstitialAdShowStarted(Action<string> callback)
        {
            _onInterstitialAdShowStartedCallback = callback;
            return this;
        }

        public IAdService Build()
        {
            var adServiceConfig = AdServiceConfig.Instance;

            ValidateConfiguration(adServiceConfig);

            var adService = new IronSourceAdService(adServiceConfig, _adImpressionTracker);

            if (_onInitializedCallback != null)
                adService.OnInitialized += _onInitializedCallback;

            if (_onInitFailedCallback != null)
                adService.OnInitializationFailed += _onInitFailedCallback;

            if (_onRewardedAdAvailabilityChangedCallback != null)
                adService.OnRewardedAdAvailabilityChanged += _onRewardedAdAvailabilityChangedCallback;

            if (_onRewardedAdShowStartedCallback != null)
                adService.OnRewardedAdShowStarted += _onRewardedAdShowStartedCallback;

            if (_onRewardedAdRewardedCallback != null)
                adService.OnRewardedAdRewarded += _onRewardedAdRewardedCallback;

            if (_onInterstitialAdShowStartedCallback != null)
                adService.OnInterstitialAdShowStarted += _onInterstitialAdShowStartedCallback;

            return adService;
        }

        private void ValidateConfiguration(AdServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.AppKey))
                Debug.LogError("[IronSourceAdServiceBuilder::ValidateConfiguration]" +
                               " App key is empty in the configuration.");

            if (string.IsNullOrEmpty(config.RewardedAdUnitId))
                Debug.LogError("[IronSourceAdServiceBuilder::ValidateConfiguration]" +
                               " Rewarded ad unit ID is empty in the configuration.");

            if (string.IsNullOrEmpty(config.InterstitialAdUnitId))
                Debug.LogError("[IronSourceAdServiceBuilder::ValidateConfiguration]" +
                               " Interstitial ad unit ID is empty in the configuration.");
        }
    }
}