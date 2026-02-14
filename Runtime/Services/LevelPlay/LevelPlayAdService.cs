using System;
using System.Diagnostics;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using JetBrains.Annotations;
using R3;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdsIntegration.Runtime.Services.IronSource
{
    [PublicAPI]
    public sealed class LevelPlayAdService : IAdService, IDisposable
    {
        private readonly AdServiceConfig _config;
        private readonly IAdImpressionTracker _adImpressionTracker;

        private IAdInitializer _adInitializer;
        private IRewardedAdService _rewardedAdService;
        private IInterstitialAdService _interstitialAdService;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        private readonly ReactiveProperty<bool> _isRewardedAvailable = new();

        private IDisposable _initializedSubscription;
        private IDisposable _isAvailableSubscription;

        internal LevelPlayAdService(AdServiceConfig config, IAdImpressionTracker adImpressionTracker)
        {
            _config = config;
            _adImpressionTracker = adImpressionTracker;
        }

        public void Init()
        {
            Logger.Log("[LevelPlayAdService::Init] Initializing ad service");

            _adInitializer = new LevelPlayInitializer(_config.AppKey);
            _rewardedAdService = new LevelPlayRewardedAdService(_adInitializer, _config);
            _interstitialAdService = new LevelPlayInterstitialAdService(_adInitializer, _config);

            _initializedSubscription = _adInitializer.OnInitializationCompleted
                .Subscribe(this, static (_, self) => self.EnableTestMode());

            _isAvailableSubscription = _rewardedAdService.IsAvailable
                .Subscribe(this, static (isAvailable, self) => self.HandleRewardedAdStatusChanged(isAvailable));

            SceneManager.sceneLoaded += OnSceneLoaded;

            Application.focusChanged += OnApplicationFocusChanged;

            LevelPlay.OnImpressionDataReady += ImpressionDataReadyEvent;

            _adInitializer.Init();
        }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
            => ShowRewardedAd(placement.GetPlacementName(), onRewarded);

        public bool ShowRewardedAd(string placementName, Action onRewarded)
        {
            if (_adInitializer.IsInitialized is false || IsRewardedAdAvailable() is false)
            {
                Logger.LogWarning("[LevelPlayAdService::ShowRewardedAd] Cannot show rewarded ad. Initialized:" +
                                  $" {_adInitializer.IsInitialized}, Ad ready: {IsRewardedAdAvailable()}");
                return false;
            }

            Logger.Log("[LevelPlayAdService::ShowRewardedAd] Showing rewarded ad for placement: {placementName}");

            _rewardedAdService.ShowAd(placementName, onRewarded);

            return true;
        }

        public bool IsRewardedAdAvailable() => _rewardedAdService != null && _rewardedAdService.IsAdReady();

        public bool TryShowInterstitial()
        {
            if (_interstitialAdService.CanShowAd() is false)
                return false;

            Logger.Log("[LevelPlayAdService::TryShowInterstitial] Showing interstitial ad");

            _interstitialAdService.ShowAd();
            return true;
        }

        private void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
        {
            Logger.Log("[LevelPlayAdService::ImpressionDataReadyEvent] " +
                       $"ImpressionDataReadyEvent impressionData = {impressionData}");

            if (impressionData == null)
                return;

            _adImpressionTracker?.TrackAdImpression(impressionData);
        }

        private void HandleRewardedAdStatusChanged(bool available)
        {
            Logger.Log("[LevelPlayAdService::HandleRewardedAdStatusChanged] " +
                       $"Rewarded ad availability changed: {available}");

            _isRewardedAvailable.OnNext(available);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_rewardedAdService.IsAdReady() is false)
                _rewardedAdService.LoadAd();

            if (_interstitialAdService.IsAdReady() is false)
                _interstitialAdService.LoadAd();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            LevelPlay.SetPauseGame(hasFocus is false);

            if (hasFocus is false)
                return;

            if (_rewardedAdService.IsAdReady() is false)
                _rewardedAdService.LoadAd();

            if (_interstitialAdService.IsAdReady() is false)
                _interstitialAdService.LoadAd();
        }

        [Conditional("ADS_TEST_MODE")]
        private void EnableTestMode()
        {
            if (_adInitializer.IsInitialized)
            {
                Logger.Log("[LevelPlayAdService::EnableTestMode] Launching test suite");

                LevelPlay.LaunchTestSuite();
                return;
            }

            Logger.LogWarning("[LevelPlayAdService::EnableTestMode] Cannot enable test mode - SDK not initialized");
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.focusChanged -= OnApplicationFocusChanged;
            LevelPlay.OnImpressionDataReady -= ImpressionDataReadyEvent;

            _adInitializer?.Dispose();
            _rewardedAdService?.Dispose();
            _interstitialAdService?.Dispose();
            _adImpressionTracker?.Dispose();
            _initializedSubscription?.Dispose();
            _isAvailableSubscription?.Dispose();
        }
    }
}