using System;

namespace AdsIntegration.Runtime.Base
{
    internal interface IAdInitializer : IDisposable
    {
        event Action OnInitializationCompleted;
        event Action<string> OnInitializationFailed;
        void Init();
        bool IsInitialized { get; }
    }
}