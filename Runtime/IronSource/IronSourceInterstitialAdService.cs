using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using ImprovedTimers;
using PrimeTween;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.IronSource
{
    internal sealed class IronSourceInterstitialAdService : IInterstitialAdService
    {
        public event Action<string> OnInterstitialAdShowStarted;

        private readonly IAdInitializer _adInitializer;
        private readonly AdServiceConfig _config;

        private LevelPlayInterstitialAd _interstitialAd;
        private CountdownTimer _interstitialTimer;
        private bool _isAdLoading;
        private int _loadAttemptCount;

        private readonly IDisposable _initializationSubscription;

        public IronSourceInterstitialAdService(IAdInitializer adInitializer, AdServiceConfig config)
        {
            _adInitializer = adInitializer;
            _config = config;

            _initializationSubscription = _adInitializer.OnInitializationCompleted
                .Subscribe(this, static (_, self) => self.Initialize());
        }

        private void Initialize()
        {
            Logger.Log("[IronSourceInterstitialAdService::Initialize] Initializing interstitial ad service");

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
            if (_adInitializer.IsInitialized is false || IsAdReady() || _isAdLoading)
                return;

            Logger.Log("[IronSourceInterstitialAdService::LoadAd] Starting to load interstitial ad");

            _isAdLoading = true;
            _interstitialAd.LoadAd();
        }

        public bool IsAdReady() => _interstitialAd != null && _interstitialAd.IsAdReady();

        public bool CanShowAd()
        {
            var canShow = _adInitializer.IsInitialized && IsAdReady() && _interstitialTimer is { IsRunning: false };

            if (canShow is false)
                Logger.Log("[IronSourceInterstitialAdService::CanShowAd] Interstitial cannot be shown: " +
                           $"Initialized: {_adInitializer.IsInitialized}, " +
                           $"Ad ready: {IsAdReady()}, " +
                           $"Timer running: {_interstitialTimer?.IsRunning}, " +
                           $"Remaining time: {_interstitialTimer?.CurrentTime}");

            return canShow;
        }

        public void ShowAd()
        {
            Logger.Log("[IronSourceInterstitialAdService::ShowAd] Showing interstitial ad");

            _interstitialAd.ShowAd();
            _interstitialTimer.Reset();
            _interstitialTimer.Start();
        }

        private void OnInterstitialAdDisplayFailed(LevelPlayAdInfo levelPlayAdInfo, LevelPlayAdError displayError)
        {
            Logger.LogError("[IronSourceInterstitialAdService::OnInterstitialAdDisplayFailed] " +
                            $"Interstitial ad display failed with {levelPlayAdInfo} info and " +
                            $"{displayError.ErrorMessage} error");
        }

        private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
        {
            Logger.Log("[IronSourceInterstitialAdService::OnInterstitialAdLoaded] " +
                       $"Interstitial ad loaded successfully, " +
                       $"unit: {adInfo.AdUnitName}, " +
                       $"placement: {adInfo.PlacementName}");

            _isAdLoading = false;
            _loadAttemptCount = 0;
        }

        private void OnInterstitialAdLoadFailed(LevelPlayAdError adError)
        {
            Logger.LogError("[IronSourceInterstitialAdService::OnInterstitialAdLoadFailed] " +
                            $"Interstitial ad load failed: {adError.ErrorMessage}, unit: {adError.AdUnitId}");

            _isAdLoading = false;

            if (_loadAttemptCount >= _config.MaxInterstitialLoadAttempts)
                return;

            Tween.Delay(this, _config.RetryLoadDelay, static service => service.LoadAd());

            _loadAttemptCount++;
        }

        private void OnInterstitialAdClosed(LevelPlayAdInfo adInfo)
        {
            Logger.Log($"[IronSourceInterstitialAdService::OnInterstitialAdClosed] " +
                       $"Interstitial ad closed, unit: {adInfo.AdUnitName}");

            LoadAd();
        }

        public void Dispose()
        {
            _interstitialTimer?.Dispose();

            if (_interstitialAd == null)
                return;

            _interstitialAd.OnAdDisplayFailed -= OnInterstitialAdDisplayFailed;
            _interstitialAd.OnAdLoaded -= OnInterstitialAdLoaded;
            _interstitialAd.OnAdLoadFailed -= OnInterstitialAdLoadFailed;
            _interstitialAd.OnAdClosed -= OnInterstitialAdClosed;

            _interstitialAd.Dispose();
            _initializationSubscription.Dispose();

            Logger.Log("[IronSourceInterstitialAdService::Dispose] Disposed");
        }
    }
}