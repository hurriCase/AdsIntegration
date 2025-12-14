using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Config;
using PrimeTween;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.IronSource
{
    internal sealed class IronSourceRewardedAdService : IRewardedAdService
    {
        public event Action<bool> OnAdStatusChanged;
        public event Action<string> OnRewardedAdShowStarted;
        public event Action<string> OnRewardedAdRewarded;

        private readonly IAdInitializer _adInitializer;
        private readonly AdServiceConfig _config;

        private Action _currentRewardCallback;

        private LevelPlayRewardedAd _rewardedAd;
        private bool _isAdLoading;
        private int _loadAttemptCount;

        private IDisposable _initializationSubscription;

        public IronSourceRewardedAdService(IAdInitializer adInitializer, AdServiceConfig config)
        {
            _adInitializer = adInitializer;
            _config = config;

            _initializationSubscription = _adInitializer.OnInitializationCompleted
                .Subscribe(this, static (_, self) => self.Initialize());
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

            Logger.Log("[IronSourceRewardedAdService::LoadAd] Starting to load rewarded ad");

            _isAdLoading = true;
            _rewardedAd.LoadAd();
        }

        public bool IsAdReady() => _rewardedAd != null && _rewardedAd.IsAdReady();

        public void ShowAd(string placementName, Action callback)
        {
            if (_adInitializer.IsInitialized is false || IsAdReady() is false)
            {
                Logger.LogWarning("[IronSourceRewardedAdService::ShowAd] Cannot show rewarded ad. Initialized: " +
                                  $"{_adInitializer.IsInitialized}, Ad ready: {IsAdReady()}");

                return;
            }

            Logger.Log($"[IronSourceRewardedAdService::ShowAd] Showing rewarded ad with placement: {placementName}");

            _currentRewardCallback = callback;

            _rewardedAd.ShowAd();

            OnRewardedAdShowStarted?.Invoke(placementName);
        }

        private void OnRewardAdClosed(LevelPlayAdInfo levelPlayAdInfo)
        {
            Logger.Log($"[IronSourceRewardedAdService::OnRewardAdClosed] " +
                       $"Rewarded ad closed, unit: {levelPlayAdInfo.AdUnitName}");

            _currentRewardCallback = null;

            OnAdStatusChanged?.Invoke(false);

            LoadAd();
        }

        private void OnRewardAdLoadFailed(LevelPlayAdError levelPlayAdError)
        {
            Logger.LogError($"[IronSourceRewardedAdService::OnRewardAdLoadFailed] " +
                            $"Rewarded ad load failed: {levelPlayAdError.ErrorMessage}," +
                            $" unit: {levelPlayAdError.AdUnitId}");

            _isAdLoading = false;

            OnAdStatusChanged?.Invoke(false);

            if (_loadAttemptCount >= _config.MaxRewardedLoadAttempts)
                return;

            Tween.Delay(this, _config.RetryLoadDelay, static service => service.LoadAd());

            _loadAttemptCount++;
        }

        private void OnRewardAdLoaded(LevelPlayAdInfo levelPlayAdInfo)
        {
            Logger.Log("[IronSourceRewardedAdService::OnRewardAdLoaded] Rewarded ad loaded successfully, " +
                       $"unit: {levelPlayAdInfo.AdUnitName}, placement: {levelPlayAdInfo.PlacementName}");

            _isAdLoading = false;
            _loadAttemptCount = 0;

            OnAdStatusChanged?.Invoke(true);
        }

        private void OnRewardAdDisplayFailed(LevelPlayAdInfo levelPlayAdInfo, LevelPlayAdError displayError)
        {
            Logger.LogError($"[IronSourceRewardedAdService::OnRewardAdDisplayFailed] " +
                            $"Rewarded ad display failed with {levelPlayAdInfo} info and " +
                            $"{displayError.ErrorMessage} error");

            _currentRewardCallback = null;
        }

        private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            Logger.Log($"[IronSourceRewardedAdService::OnAdRewarded] " +
                       $"Reward granted: {reward.Name}, amount: {reward.Amount}");

            if (_currentRewardCallback is null)
            {
                Logger.LogWarning("[IronSourceRewardedAdService::OnAdRewarded] " +
                                  "Reward callback was null when reward was granted");
                return;
            }

            _currentRewardCallback.Invoke();
            _currentRewardCallback = null;

            OnRewardedAdRewarded?.Invoke(adInfo.PlacementName);
        }

        public void Dispose()
        {
            if (_rewardedAd is null)
                return;

            _rewardedAd.OnAdDisplayFailed -= OnRewardAdDisplayFailed;
            _rewardedAd.OnAdRewarded -= OnAdRewarded;
            _rewardedAd.OnAdLoaded -= OnRewardAdLoaded;
            _rewardedAd.OnAdLoadFailed -= OnRewardAdLoadFailed;
            _rewardedAd.OnAdClosed -= OnRewardAdClosed;

            _rewardedAd.Dispose();
            _initializationSubscription.Dispose();

            Logger.Log("[IronSourceRewardedAdService::Dispose] Disposed");
        }
    }
}