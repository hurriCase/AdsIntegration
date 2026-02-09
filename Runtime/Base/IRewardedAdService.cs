using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    internal interface IRewardedAdService : IDisposable
    {
        ReadOnlyReactiveProperty<bool> IsAvailable { get; }
        void LoadAd();
        bool IsAdReady();
        void ShowAd(string placementName, Action callback);
    }
}