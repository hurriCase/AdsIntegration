using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AdsIntegration.Runtime
{
    /// <summary>
    /// Implementation of IAdService using IronSource SDK
    /// </summary>
    public sealed class IronSourceAdService : IAdService, IDisposable
    {
        public event Action<bool> OnRewardedAdAvailabilityChanged;
        public event Action OnInitialized;
        public event Action<string> OnInitializationFailed;

        private readonly AdServiceConfig _config;
        private readonly bool _debugLogging;
        private readonly bool _testMode;

        private IAdInitializer _adInitializer;
        private IRewardedAdService _rewardedAdService;
        private IInterstitialAdService _interstitialAdService;

        internal IronSourceAdService(AdServiceConfig config, bool debugLogging, bool testMode)
        {
            _config = config;
            _debugLogging = debugLogging;
            _testMode = testMode;
        }

        public void Initialize()
        {
            if (_debugLogging)
                Debug.Log("[IronSourceAdService] Initializing ad service");

            _adInitializer = new IronSourceInitializer(_config.AppKey, _debugLogging);
            _rewardedAdService = new IronSourceRewardedAdService(_adInitializer, _config, _debugLogging);
            _interstitialAdService = new IronSourceInterstitialAdService(_adInitializer, _config, _debugLogging);

            _adInitializer.OnInitializationCompleted += HandleInitializationCompleted;
            _adInitializer.OnInitializationFailed += HandleInitializationFailed;

            _rewardedAdService.OnAdStatusChanged += HandleRewardedAdStatusChanged;

            SceneManager.sceneLoaded += OnSceneLoaded;

            Application.focusChanged += OnApplicationFocusChanged;

            _adInitializer.Init();

            if (_testMode && _debugLogging)
                Debug.Log("[IronSourceAdService] Test mode enabled");
        }

        private void HandleInitializationCompleted()
        {
            if (_debugLogging)
                Debug.Log("[IronSourceAdService] Initialization completed");

            OnInitialized?.Invoke();

            if (_testMode)
                EnableTestMode();
        }

        private void HandleInitializationFailed(string errorMessage)
        {
            if (_debugLogging)
                Debug.LogError($"[IronSourceAdService] Initialization failed: {errorMessage}");

            OnInitializationFailed?.Invoke(errorMessage);
        }

        private void HandleRewardedAdStatusChanged(bool available)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceAdService] Rewarded ad availability changed: {available}");

            OnRewardedAdAvailabilityChanged?.Invoke(available);
        }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
        {
            string placementName = placement.GetPlacementName();
            return ShowRewardedAd(placementName, onRewarded);
        }

        public bool ShowRewardedAd(string placementName, Action onRewarded)
        {
            if (_adInitializer.IsInitialized is false || IsRewardedAdAvailable() is false)
            {
                if (_debugLogging)
                    Debug.LogWarning($"[IronSourceAdService] Cannot show rewarded ad. Initialized: {_adInitializer.IsInitialized}, Ad ready: {IsRewardedAdAvailable()}");
                return false;
            }

            var rewardType = FindRewardTypeForPlacement(placementName);

            if (_debugLogging)
                Debug.Log($"[IronSourceAdService] Showing rewarded ad for placement: {placementName}, reward type: {rewardType}");

            _rewardedAdService.ShowAd(placementName, rewardType, onRewarded);
            return true;
        }

        private string FindRewardTypeForPlacement(string placementName)
        {
            return _config.FindRewardTypeForPlacement(placementName);
        }

        public bool IsRewardedAdAvailable() => _rewardedAdService != null && _rewardedAdService.IsAdReady();

        public bool TryShowInterstitial()
        {
            if (_interstitialAdService.CanShowAd() is false)
                return false;

            if (_debugLogging)
                Debug.Log("[IronSourceAdService] Showing interstitial ad");

            _interstitialAdService.ShowAd();
            return true;
        }

        public void EnableTestMode()
        {
            if (_adInitializer.IsInitialized)
            {
                if (_debugLogging)
                    Debug.Log("[IronSourceAdService] Launching test suite");

                IronSource.Agent.launchTestSuite();
            }
            else if (_debugLogging)
                Debug.LogWarning("[IronSourceAdService] Cannot enable test mode - SDK not initialized");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceAdService] Scene loaded: {scene.name}, reloading ads if needed");

            if (_rewardedAdService.IsAdReady() is false)
                _rewardedAdService.LoadAd();

            if (_interstitialAdService.IsAdReady() is false)
                _interstitialAdService.LoadAd();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceAdService] Application focus changed: {hasFocus}");

            IronSource.Agent.onApplicationPause(hasFocus is false);

            if (hasFocus is false)
                return;

            if (_rewardedAdService.IsAdReady() is false)
                _rewardedAdService.LoadAd();

            if (_interstitialAdService.IsAdReady() is false)
                _interstitialAdService.LoadAd();
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.focusChanged -= OnApplicationFocusChanged;

            _adInitializer.OnInitializationCompleted -= HandleInitializationCompleted;
            _adInitializer.OnInitializationFailed -= HandleInitializationFailed;
            _rewardedAdService.OnAdStatusChanged -= HandleRewardedAdStatusChanged;

            _adInitializer?.Dispose();
            _rewardedAdService?.Dispose();
            _interstitialAdService?.Dispose();

            if (_debugLogging)
                Debug.Log("[IronSourceAdService] Disposed");
        }
    }
}