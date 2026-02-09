using System;

namespace AdsIntegration.Runtime.Base
{
    internal interface IInterstitialAdService : IDisposable
    {
        void LoadAd();
        bool IsAdReady();
        bool CanShowAd();
        void ShowAd();
    }
}