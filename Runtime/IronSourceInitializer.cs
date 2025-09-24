using System;
using AdsIntegration.Runtime.Base;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime
{
    internal sealed class IronSourceInitializer : IAdInitializer
    {
        public event Action OnInitializationCompleted;
        public event Action<string> OnInitializationFailed;

        public bool IsInitialized { get; private set; }

        private readonly string _appKey;

        public IronSourceInitializer(string appKey)
        {
            _appKey = appKey;
        }

        public void Init()
        {
            Logger.Log($"[IronSourceInitializer::Init] Initializing IronSource SDK with app key: {_appKey}");

            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

            LevelPlay.Init(_appKey);
        }

        private void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
        {
            IsInitialized = true;

            Logger.Log("[IronSourceInitializer::SdkInitializationCompletedEvent] SDK initialized successfully");

            OnInitializationCompleted?.Invoke();
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;

            Logger.LogError($"[IronSourceInitializer::SdkInitializationFailedEvent] " +
                            $"SDK initialization failed: {levelPlayInitError.ErrorMessage}");

            OnInitializationFailed?.Invoke(levelPlayInitError.ErrorMessage);
        }

        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;

            Logger.Log("[IronSourceInitializer::Dispose] Disposed");
        }
    }
}