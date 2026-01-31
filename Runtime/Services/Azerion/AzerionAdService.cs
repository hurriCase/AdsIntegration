#if AZERION
using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdsIntegration.Runtime.Services.Azerion
{
    [PublicAPI]
    public sealed class AzerionAdService : IAdService, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);

        private Action _onRewarded;

        public void Init()
        {
            var gameDistribution = ResourceLoader<GameDistribution>.Load(nameof(GameDistribution));
            Object.Instantiate(gameDistribution);

            GameDistribution.Instance.GAME_KEY = AzerionConfig.Instance.GameKey;

            GameDistribution.Instance.PreloadRewardedAd();

            GameDistribution.OnResumeGame += ResumeGame;
            GameDistribution.OnPauseGame += PauseGame;
            GameDistribution.OnRewardedVideoSuccess += OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure += OnRewardedAdDisplayFailed;
        }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
        {
            Logger.Log("[AzerionAdService::ShowRewardedAd] Showing Rewarded ad");

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
            Logger.LogError("[AzerionAdService::OnRewardedAdDisplayFailed] Rewarded ad display failed");
        }

        private void OnRewardedAdFinished()
        {
            Logger.Log("[AzerionAdService::OnRewardedAdFinished] Rewarded successfully finished");

            _onRewarded?.Invoke();

            GameDistribution.Instance.PreloadRewardedAd();
        }

        private void OnInterstitialAdStarted()
        {
            Logger.Log("[AzerionAdService::OnInterstitialAdStarted] Showing interstitial ad");
        }

        private void OnInterstitialAdFinished()
        {
            Logger.Log("[AzerionAdService::OnInterstitialAdFinished] Interstitial successfully finished");
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
            _isRewardedAvailable.Dispose();

            GameDistribution.OnResumeGame -= ResumeGame;
            GameDistribution.OnPauseGame -= PauseGame;
            GameDistribution.OnRewardedVideoSuccess -= OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure -= OnRewardedAdDisplayFailed;
        }
    }
}
#endif