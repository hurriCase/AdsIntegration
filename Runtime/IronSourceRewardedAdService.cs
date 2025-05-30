﻿using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using PrimeTween;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace AdsIntegration.Runtime
{
    internal sealed class IronSourceRewardedAdService : IRewardedAdService
    {
        public event Action<bool> OnAdStatusChanged;
        public event Action<string> OnRewardedAdShowStarted;
        public event Action<string> OnRewardedAdRewarded;

        private readonly IAdInitializer _adInitializer;
        private readonly AdServiceConfig _config;
        private readonly bool _debugLogging;

        private Action _currentRewardCallback;

        private LevelPlayRewardedAd _rewardedAd;
        private bool _isAdLoading;
        private int _loadAttemptCount;

        public IronSourceRewardedAdService(IAdInitializer adInitializer, AdServiceConfig config, bool debugLogging)
        {
            _adInitializer = adInitializer;
            _config = config;
            _debugLogging = debugLogging;

            _adInitializer.OnInitializationCompleted += Initialize;
        }

        private void Initialize()
        {
            _rewardedAd = new LevelPlayRewardedAd(_config.RewardedAdUnitId);

            _rewardedAd.OnAdDisplayFailed += OnRewardAdDisplayFailed;
            _rewardedAd.OnAdRewarded += OnAdRewarded;
            _rewardedAd.OnAdLoaded += OnRewardAdLoaded;
            _rewardedAd.OnAdLoadFailed += OnRewardAdLoadFailed;
            _rewardedAd.OnAdClosed += OnRewardAdClosed;

            LoadAd();
        }

        public void LoadAd()
        {
            if (_adInitializer.IsInitialized is false || IsAdReady() || _isAdLoading)
                return;

            if (_debugLogging)
                Debug.Log("[IronSourceRewardedAdService] Starting to load rewarded ad");

            _isAdLoading = true;
            _rewardedAd.LoadAd();
        }

        public bool IsAdReady() => _rewardedAd != null && _rewardedAd.IsAdReady();

        public void ShowAd(string placementName, Action callback)
        {
            if (_adInitializer.IsInitialized is false || IsAdReady() is false)
            {
                if (_debugLogging)
                    Debug.LogWarning(
                        "[IronSourceRewardedAdService] Cannot show rewarded ad. Initialized: " +
                        $"{_adInitializer.IsInitialized}, Ad ready: {IsAdReady()}");

                return;
            }

            if (_debugLogging)
                Debug.Log(
                    $"[IronSourceRewardedAdService] Showing rewarded ad with placement: {placementName}");

            _currentRewardCallback = callback;

            _rewardedAd.ShowAd();

            OnRewardedAdShowStarted?.Invoke(placementName);
        }

        private void OnRewardAdClosed(LevelPlayAdInfo levelPlayAdInfo)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceRewardedAdService] Rewarded ad closed, unit: {levelPlayAdInfo.AdUnitName}");

            _currentRewardCallback = null;

            OnAdStatusChanged?.Invoke(false);

            LoadAd();
        }

        private void OnRewardAdLoadFailed(LevelPlayAdError levelPlayAdError)
        {
            if (_debugLogging)
                Debug.LogError(
                    $"[IronSourceRewardedAdService] Rewarded ad load failed: {levelPlayAdError.ErrorMessage}," +
                    $" unit: {levelPlayAdError.AdUnitId}");

            _isAdLoading = false;

            OnAdStatusChanged?.Invoke(false);

            if (_loadAttemptCount >= _config.MaxRewardedLoadAttempts)
                return;

            Tween.Delay(this, _config.RetryLoadDelay, service => service.LoadAd());

            _loadAttemptCount++;
        }

        private void OnRewardAdLoaded(LevelPlayAdInfo levelPlayAdInfo)
        {
            if (_debugLogging)
                Debug.Log(
                    "[IronSourceRewardedAdService] Rewarded ad loaded successfully, " +
                    $"unit: {levelPlayAdInfo.AdUnitName}, placement: {levelPlayAdInfo.PlacementName}");

            _isAdLoading = false;
            _loadAttemptCount = 0;

            OnAdStatusChanged?.Invoke(true);
        }

        private void OnRewardAdDisplayFailed(LevelPlayAdDisplayInfoError displayError)
        {
            if (_debugLogging)
                Debug.LogError(
                    $"[IronSourceRewardedAdService] Rewarded ad display failed: {displayError.LevelPlayError}");

            _currentRewardCallback = null;
        }

        private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceRewardedAdService] Reward granted: {reward.Name}, amount: {reward.Amount}");

            if (_currentRewardCallback != null)
            {
                _currentRewardCallback.Invoke();
                _currentRewardCallback = null;

                OnRewardedAdRewarded?.Invoke(adInfo.PlacementName);
            }
            else if (_debugLogging)
                Debug.LogWarning("[IronSourceRewardedAdService] Reward callback was null when reward was granted");
        }

        public void Dispose()
        {
            _adInitializer.OnInitializationCompleted -= Initialize;

            if (_rewardedAd is null)
                return;

            _rewardedAd.OnAdDisplayFailed -= OnRewardAdDisplayFailed;
            _rewardedAd.OnAdRewarded -= OnAdRewarded;
            _rewardedAd.OnAdLoaded -= OnRewardAdLoaded;
            _rewardedAd.OnAdLoadFailed -= OnRewardAdLoadFailed;
            _rewardedAd.OnAdClosed -= OnRewardAdClosed;

            _rewardedAd.Dispose();

            if (_debugLogging)
                Debug.Log("[IronSourceRewardedAdService] Disposed");
        }
    }
}