using System;

namespace AdsIntegration.Runtime.Base
{
    internal interface IInterstitialAdService : IDisposable
    {
        event Action<string> OnInterstitialAdShowStarted;

        void LoadAd();
        bool IsAdReady();
        bool CanShowAd();
        void ShowAd();
    }
}