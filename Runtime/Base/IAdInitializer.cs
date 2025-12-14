using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    internal interface IAdInitializer : IDisposable
    {
        Observable<Unit> OnInitializationCompleted { get; }
        void Init();
        bool IsInitialized { get; }
    }
}