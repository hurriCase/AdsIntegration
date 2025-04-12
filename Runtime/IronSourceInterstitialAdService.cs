using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using ImprovedTimers;
using PrimeTween;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace AdsIntegration.Runtime
{
    internal sealed class IronSourceInterstitialAdService : IInterstitialAdService
    {
        private readonly IAdInitializer _adInitializer;
        private readonly AdServiceConfig _config;
        private readonly bool _debugLogging;

        private LevelPlayInterstitialAd _interstitialAd;
        private CountdownTimer _interstitialTimer;
        private bool _isAdLoading;
        private int _loadAttemptCount;

        public IronSourceInterstitialAdService(IAdInitializer adInitializer, AdServiceConfig config, bool debugLogging)
        {
            _adInitializer = adInitializer;
            _config = config;
            _debugLogging = debugLogging;

            _adInitializer.OnInitializationCompleted += Initialize;
        }

        private void Initialize()
        {
            if (_debugLogging)
                Debug.Log("[IronSourceInterstitialAdService] Initializing interstitial ad service");

            _interstitialTimer = new CountdownTimer(_config.TimeBetweenInterstitials);
            _interstitialAd = new LevelPlayInterstitialAd(_config.InterstitialAdUnitId);

            _interstitialAd.OnAdDisplayFailed += OnInterstitialAdDisplayFailed;
            _interstitialAd.OnAdLoaded += OnInterstitialAdLoaded;
            _interstitialAd.OnAdLoadFailed += OnInterstitialAdLoadFailed;
            _interstitialAd.OnAdClosed += OnInterstitialAdClosed;

            LoadAd();
        }

        public void LoadAd()
        {
            if (_adInitializer.IsInitialized == false || IsAdReady() || _isAdLoading)
                return;

            if (_debugLogging)
                Debug.Log("[IronSourceInterstitialAdService] Starting to load interstitial ad");

            _isAdLoading = true;
            _interstitialAd.LoadAd();
        }

        public bool IsAdReady() => _interstitialAd != null && _interstitialAd.IsAdReady();

        public bool CanShowAd()
        {
            var canShow = _adInitializer.IsInitialized &&
                          IsAdReady() &&
                          _interstitialTimer is { IsRunning: false };

            if (canShow == false && _debugLogging)
                Debug.Log("[IronSourceInterstitialAdService] Interstitial cannot be shown: " +
                          $"Initialized: {_adInitializer.IsInitialized}, " +
                          $"Ad ready: {IsAdReady()}, " +
                          $"Timer running: {_interstitialTimer?.IsRunning}, " +
                          $"Remaining time: {_interstitialTimer?.CurrentTime}");

            return canShow;
        }

        public void ShowAd()
        {
            if (_debugLogging)
                Debug.Log("[IronSourceInterstitialAdService] Showing interstitial ad");

            _interstitialAd.ShowAd();
            _interstitialTimer.Reset();
            _interstitialTimer.Start();
        }

        private void OnInterstitialAdDisplayFailed(LevelPlayAdDisplayInfoError displayError)
        {
            if (_debugLogging)
                Debug.LogError(
                    $"[IronSourceInterstitialAdService] Interstitial ad display failed: {displayError.LevelPlayError}");
        }

        private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (_debugLogging)
                Debug.Log(
                    $"[IronSourceInterstitialAdService] Interstitial ad loaded successfully, unit: {adInfo.AdUnitName}, placement: {adInfo.PlacementName}");

            _isAdLoading = false;
            _loadAttemptCount = 0;
        }

        private void OnInterstitialAdLoadFailed(LevelPlayAdError adError)
        {
            if (_debugLogging)
                Debug.LogError(
                    $"[IronSourceInterstitialAdService] Interstitial ad load failed: {adError.ErrorMessage}, unit: {adError.AdUnitId}");

            _isAdLoading = false;

            if (_loadAttemptCount >= _config.MaxInterstitialLoadAttempts)
                return;

            Tween.Delay(this, _config.RetryLoadDelay, service => service.LoadAd());

            _loadAttemptCount++;
        }

        private void OnInterstitialAdClosed(LevelPlayAdInfo adInfo)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceInterstitialAdService] Interstitial ad closed, unit: {adInfo.AdUnitName}");

            LoadAd();
        }

        public void Dispose()
        {
            _adInitializer.OnInitializationCompleted -= Initialize;

            _interstitialTimer?.Dispose();

            if (_interstitialAd == null)
                return;

            _interstitialAd.OnAdDisplayFailed -= OnInterstitialAdDisplayFailed;
            _interstitialAd.OnAdLoaded -= OnInterstitialAdLoaded;
            _interstitialAd.OnAdLoadFailed -= OnInterstitialAdLoadFailed;
            _interstitialAd.OnAdClosed -= OnInterstitialAdClosed;

            _interstitialAd.Dispose();

            if (_debugLogging)
                Debug.Log("[IronSourceInterstitialAdService] Disposed");
        }
    }
}