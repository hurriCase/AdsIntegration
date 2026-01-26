using System;
using AdsIntegration.Runtime.Base;
using CrazyGames;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdsIntegration.Runtime.Services.Azerion
{
    [PublicAPI]
    public sealed class AzerionAdService : IAdService, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _rewardedAdAvailabilityChanged;
        private readonly ReactiveProperty<bool> _rewardedAdAvailabilityChanged = new(true);

        private Action _onRewarded;

        public void Init()
        {
            Object.Instantiate(AzerionConfig.Instance.GameDistributionPrefab);

            GameDistribution.Instance.GAME_KEY = AzerionConfig.Instance.GameKey;

            GameDistribution.Instance.PreloadRewardedAd();

            GameDistribution.OnResumeGame += ResumeGame;
            GameDistribution.OnPauseGame += PauseGame;
            GameDistribution.OnRewardedVideoSuccess += OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure += OnRewardedAdDisplayFailed;
        }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
        {
            Logger.Log("[CrazyGamesAdService::OnRewardedAdStarted] Showing Rewarded ad");

            _onRewarded = onRewarded;

            GameDistribution.Instance.ShowRewardedAd();

            return true;
        }

        public bool TryShowInterstitial()
        {
            GameDistribution.Instance.ShowAd();
            return true;
        }

        private void OnRewardedAdDisplayFailed()
        {
            Logger.LogError("[CrazyGamesAdService::OnRewardedAdDisplayFailed] Rewarded ad display failed");
        }

        private void OnRewardedAdFinished()
        {
            Logger.Log("[CrazyGamesAdService::OnRewardedAdFinished] Rewarded successfully finished");

            _onRewarded?.Invoke();

            GameDistribution.Instance.PreloadRewardedAd();
        }

        private void OnInterstitialAdStarted()
        {
            Logger.Log("[CrazyGamesAdService::OnInterstitialAdStarted] Showing interstitial ad");
        }

        private void OnInterstitialAdDisplayFailed(SdkError error)
        {
            Logger.LogError("[CrazyGamesAdService::OnInterstitialAdDisplayFailed] " +
                            $"Interstitial ad display failed with {error.message} error");
        }

        private void OnInterstitialAdFinished()
        {
            Logger.Log("[CrazyGamesAdService::OnInterstitialAdFinished] Interstitial successfully finished");
        }

        private void ResumeGame()
        {
            Time.timeScale = 1;
        }

        private void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void Dispose()
        {
            _rewardedAdAvailabilityChanged.Dispose();

            GameDistribution.OnResumeGame -= ResumeGame;
            GameDistribution.OnPauseGame -= PauseGame;
            GameDistribution.OnRewardedVideoSuccess -= OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure -= OnRewardedAdDisplayFailed;
        }
    }
}