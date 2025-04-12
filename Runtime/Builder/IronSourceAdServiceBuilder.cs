using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using UnityEngine;

namespace AdsIntegration.Runtime.Builder
{
    /// <summary>
    /// Builder implementation for creating IronSource ad service instances
    /// </summary>
    public sealed class IronSourceAdServiceBuilder : IAdServiceBuilder
    {
        private bool _debugLogging;
        private bool _testMode;
        private Action _onInitializedCallback;
        private Action<string> _onInitFailedCallback;
        private Action<bool> _onRewardedAdAvailabilityChangedCallback;

        public IAdServiceBuilder WithDebugLogging()
        {
            _debugLogging = true;
            return this;
        }

        public IAdServiceBuilder WithTestMode()
        {
            _testMode = true;
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

        public IAdService Build()
        {
            var config = AdServiceConfig.GetOrCreateSettings();

            ValidateConfiguration(config);

            var adService = new IronSourceAdService(config, _debugLogging, _testMode);

            if (_onInitializedCallback != null)
                adService.OnInitialized += _onInitializedCallback;

            if (_onInitFailedCallback != null)
                adService.OnInitializationFailed += _onInitFailedCallback;

            if (_onRewardedAdAvailabilityChangedCallback != null)
                adService.OnRewardedAdAvailabilityChanged += _onRewardedAdAvailabilityChangedCallback;

            return adService;
        }

        private void ValidateConfiguration(AdServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.AppKey))
                Debug.LogError("[IronSourceAdServiceBuilder] App key is empty in the configuration.");

            if (string.IsNullOrEmpty(config.RewardedAdUnitId))
                Debug.LogError("[IronSourceAdServiceBuilder] Rewarded ad unit ID is empty in the configuration.");

            if (string.IsNullOrEmpty(config.InterstitialAdUnitId))
                Debug.LogError("[IronSourceAdServiceBuilder] Interstitial ad unit ID is empty in the configuration.");
        }
    }
}