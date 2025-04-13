using System;

namespace AdsIntegration.Runtime.Base
{
    internal interface IRewardedAdService : IDisposable
    {
        event Action<bool> OnAdStatusChanged;
        event Action<string> OnRewardedAdShowStarted;
        event Action<string> OnRewardedAdRewarded;
        void LoadAd();
        bool IsAdReady();
        void ShowAd(string placementName, Action callback);
    }
}