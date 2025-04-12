using System;

namespace AdsIntegration.Runtime.Base
{
    internal interface IRewardedAdService : IDisposable
    {
        event Action<bool> OnAdStatusChanged;
        void LoadAd();
        bool IsAdReady();
        void ShowAd(string placementName, string rewardType, Action callback);
    }
}