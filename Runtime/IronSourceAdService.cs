using System;
using System.Diagnostics;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using JetBrains.Annotations;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdsIntegration.Runtime
{
    [UsedImplicitly]
    public sealed class IronSourceAdService : IAdService, IDisposable
    {
        public event Action OnInitialized;
        public event Action<string> OnInitializationFailed;
        public event Action<bool> OnRewardedAdAvailabilityChanged;
        public event Action<string> OnRewardedAdShowStarted;
        public event Action<string> OnRewardedAdRewarded;
        public event Action<string> OnInterstitialAdShowStarted;

        private readonly AdServiceConfig _config;
        private readonly IAdImpressionTracker _adImpressionTracker;

        private IAdInitializer _adInitializer;
        private IRewardedAdService _rewardedAdService;
        private IInterstitialAdService _interstitialAdService;

        internal IronSourceAdService(AdServiceConfig config, IAdImpressionTracker adImpressionTracker)
        {
            _config = config;
            _adImpressionTracker = adImpressionTracker;
        }

        public void Init()
        {
            Logger.Log("[IronSourceAdService::Init] Initializing ad service");

            _adInitializer = new IronSourceInitializer(_config.AppKey);
            _rewardedAdService = new IronSourceRewardedAdService(_adInitializer, _config);
            _interstitialAdService = new IronSourceInterstitialAdService(_adInitializer, _config);

            _adInitializer.OnInitializationCompleted += HandleInitializationCompleted;
            _adInitializer.OnInitializationFailed += HandleInitializationFailed;

            _rewardedAdService.OnAdStatusChanged += HandleRewardedAdStatusChanged;
            _rewardedAdService.OnRewardedAdShowStarted += HandleRewardedAdShowStarted;
            _rewardedAdService.OnRewardedAdRewarded += HandleRewardedAdRewarded;
            _interstitialAdService.OnInterstitialAdShowStarted += HandleInterstitialAdShowStarted;

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
                Logger.LogWarning("[IronSourceAdService::ShowRewardedAd] Cannot show rewarded ad. Initialized:" +
                                  $" {_adInitializer.IsInitialized}, Ad ready: {IsRewardedAdAvailable()}");
                return false;
            }

            Logger.Log("[IronSourceAdService::ShowRewardedAd] Showing rewarded ad for placement: {placementName}");

            _rewardedAdService.ShowAd(placementName, onRewarded);

            return true;
        }

        public bool IsRewardedAdAvailable() => _rewardedAdService != null && _rewardedAdService.IsAdReady();

        public bool TryShowInterstitial()
        {
            if (_interstitialAdService.CanShowAd() is false)
                return false;

            Logger.Log("[IronSourceAdService::TryShowInterstitial] Showing interstitial ad");

            _interstitialAdService.ShowAd();
            return true;
        }

        private void ImpressionDataReadyEvent(LevelPlayImpressionData impressionData)
        {
            Logger.Log("\"[IronSourceAdService::ImpressionDataReadyEvent] " +
                       $"ImpressionDataReadyEvent impressionData = {impressionData}");

            if (impressionData == null)
                return;

            _adImpressionTracker?.TrackAdImpression(impressionData);
        }

        private void HandleInitializationCompleted()
        {
            Logger.Log("[IronSourceAdService::HandleInitializationCompleted] Initialization completed");

            OnInitialized?.Invoke();

            EnableTestMode();
        }

        private void HandleInitializationFailed(string errorMessage)
        {
            Logger.LogError($"[IronSourceAdService::HandleInitializationFailed] Initialization failed: {errorMessage}");

            OnInitializationFailed?.Invoke(errorMessage);
        }

        private void HandleRewardedAdStatusChanged(bool available)
        {
            Logger.Log($"[IronSourceAdService::HandleRewardedAdStatusChanged] " +
                       $"Rewarded ad availability changed: {available}");

            OnRewardedAdAvailabilityChanged?.Invoke(available);
        }

        private void HandleRewardedAdShowStarted(string placementName)
        {
            Logger.Log($"[IronSourceAdService::HandleRewardedAdShowStarted] Rewarded ad show started: {placementName}");

            OnRewardedAdShowStarted?.Invoke(placementName);
        }

        private void HandleRewardedAdRewarded(string placementName)
        {
            Logger.Log($"[IronSourceAdService::HandleRewardedAdRewarded] Rewarded ad show ended: {placementName}");

            OnRewardedAdRewarded?.Invoke(placementName);
        }

        private void HandleInterstitialAdShowStarted(string adUnitId)
        {
            Logger.Log($"[IronSourceAdService::HandleRewardedAdRewarded] Interstitial ad show started: {adUnitId}");

            OnInterstitialAdShowStarted?.Invoke(adUnitId);
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
                Logger.Log("[IronSourceAdService::EnableTestMode] Launching test suite");

                LevelPlay.LaunchTestSuite();
                return;
            }

            Logger.LogWarning("[IronSourceAdService::EnableTestMode] Cannot enable test mode - SDK not initialized");
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.focusChanged -= OnApplicationFocusChanged;
            LevelPlay.OnImpressionDataReady -= ImpressionDataReadyEvent;

            _adInitializer.OnInitializationCompleted -= HandleInitializationCompleted;
            _adInitializer.OnInitializationFailed -= HandleInitializationFailed;
            _rewardedAdService.OnAdStatusChanged -= HandleRewardedAdStatusChanged;
            _rewardedAdService.OnRewardedAdShowStarted -= HandleRewardedAdShowStarted;
            _rewardedAdService.OnRewardedAdRewarded -= HandleRewardedAdRewarded;
            _interstitialAdService.OnInterstitialAdShowStarted -= HandleInterstitialAdShowStarted;

            _adInitializer?.Dispose();
            _rewardedAdService?.Dispose();
            _interstitialAdService?.Dispose();
            _adImpressionTracker?.Dispose();
        }
    }
}