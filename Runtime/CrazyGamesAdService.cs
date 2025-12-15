using System;
using AdsIntegration.Runtime.Base;
using CrazyGames;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime
{
    [PublicAPI]
    public sealed class CrazyGamesAdService : IAdService
    {
        public ReadOnlyReactiveProperty<bool> OnRewardedAdAvailabilityChanged => _rewardedAdAvailabilityChanged;
        private readonly ReactiveProperty<bool> _rewardedAdAvailabilityChanged = new(true);

        public void Init()
        {
            CrazySDK.Ad.HasAdblock(hasAdblock => _rewardedAdAvailabilityChanged.Value = hasAdblock is false);
        }

        public bool ShowRewardedAd(Enum placement, Action onRewarded)
        {
            CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded,
                OnRewardedAdStarted,
                OnRewardedAdDisplayFailed,
                () => OnRewardedAdFinished(onRewarded));

            return true;
        }

        public bool IsRewardedAdAvailable() => CrazySDK.Ad.AdblockStatus != AdblockStatus.Present;

        public bool TryShowInterstitial()
        {
            CrazySDK.Ad.RequestAd(
                CrazyAdType.Midgame,
                OnInterstitialAdStarted,
                OnInterstitialAdDisplayFailed,
                OnInterstitialAdFinished);

            return true;
        }

        private void OnRewardedAdStarted()
        {
            Logger.Log("[CrazyGamesAdService::OnRewardedAdStarted] Showing Rewarded ad");
        }

        private void OnRewardedAdDisplayFailed(SdkError error)
        {
            Logger.LogError("[CrazyGamesAdService::OnRewardedAdDisplayFailed] " +
                            $"Rewarded ad display failed with {error.message} error");
        }

        private void OnRewardedAdFinished(Action onRewarded)
        {
            Logger.Log("[CrazyGamesAdService::OnRewardedAdFinished] Rewarded successfully finished");

            onRewarded?.Invoke();
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
    }
}