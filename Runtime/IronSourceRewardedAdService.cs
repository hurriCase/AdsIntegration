using System;
using System.Collections.Generic;
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

        private readonly IAdInitializer _adInitializer;
        private readonly AdServiceConfig _config;
        private readonly bool _debugLogging;
        private readonly Dictionary<string, Action> _pendingRewards = new();

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
            if (_debugLogging)
                Debug.Log("[IronSourceRewardedAdService] Initializing rewarded ad service");

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

        public void ShowAd(string placementName, string rewardType, Action callback)
        {
            if (_adInitializer.IsInitialized is false || IsAdReady() is false)
            {
                if (_debugLogging)
                    Debug.LogWarning($"[IronSourceRewardedAdService] Cannot show rewarded ad. Initialized: {_adInitializer.IsInitialized}, Ad ready: {IsAdReady()}");

                return;
            }

            if (_debugLogging)
                Debug.Log($"[IronSourceRewardedAdService] Showing rewarded ad with placement: {placementName}, reward type: {rewardType}");

            _pendingRewards[rewardType] = callback;

            _rewardedAd.ShowAd(placementName);
        }

        private void OnRewardAdClosed(LevelPlayAdInfo levelPlayAdInfo)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceRewardedAdService] Rewarded ad closed, unit: {levelPlayAdInfo.AdUnitName}");

            OnAdStatusChanged?.Invoke(false);
            LoadAd();
        }

        private void OnRewardAdLoadFailed(LevelPlayAdError levelPlayAdError)
        {
            if (_debugLogging)
                Debug.LogError($"[IronSourceRewardedAdService] Rewarded ad load failed: {levelPlayAdError.ErrorMessage}, unit: {levelPlayAdError.AdUnitId}");

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
                Debug.Log($"[IronSourceRewardedAdService] Rewarded ad loaded successfully, unit: {levelPlayAdInfo.AdUnitName}, placement: {levelPlayAdInfo.PlacementName}");

            _isAdLoading = false;
            _loadAttemptCount = 0;
            OnAdStatusChanged?.Invoke(true);
        }

        private void OnRewardAdDisplayFailed(LevelPlayAdDisplayInfoError displayError)
        {
            if (_debugLogging)
                Debug.LogError($"[IronSourceRewardedAdService] Rewarded ad display failed: {displayError.LevelPlayError}");

            var rewardType = displayError.DisplayLevelPlayAdInfo.AdUnitName;

            if (_pendingRewards.ContainsKey(rewardType))
                _pendingRewards.Remove(rewardType);
        }

        private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceRewardedAdService] Reward granted: {reward.Name}, amount: {reward.Amount}");

            if (_pendingRewards.TryGetValue(reward.Name, out var callback))
            {
                callback?.Invoke();
                _pendingRewards.Remove(reward.Name);
            }
            else if (_debugLogging)
                Debug.LogWarning($"[IronSourceRewardedAdService] No callback found for reward type: {reward.Name}");
        }

        public void Dispose()
        {
            _adInitializer.OnInitializationCompleted -= Initialize;

            if (_rewardedAd == null)
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