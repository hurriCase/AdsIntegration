using System;
using AdsIntegration.Runtime.Base;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace AdsIntegration.Runtime
{
    internal sealed class IronSourceInitializer : IAdInitializer
    {
        public event Action OnInitializationCompleted;
        public event Action<string> OnInitializationFailed;

        public bool IsInitialized { get; private set; }

        private readonly string _appKey;
        private readonly bool _debugLogging;

        public IronSourceInitializer(string appKey, bool debugLogging)
        {
            _appKey = appKey;
            _debugLogging = debugLogging;
        }

        public void Init()
        {
            if (_debugLogging)
                Debug.Log($"[IronSourceInitializer] Initializing IronSource SDK with app key: {_appKey}");

            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

            LevelPlay.Init(_appKey);
        }

        private void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
        {
            IsInitialized = true;

            if (_debugLogging)
                Debug.Log("[IronSourceInitializer] SDK initialized successfully");

            OnInitializationCompleted?.Invoke();
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;

            if (_debugLogging)
                Debug.LogError($"[IronSourceInitializer] SDK initialization failed: {levelPlayInitError.ErrorMessage}");

            OnInitializationFailed?.Invoke(levelPlayInitError.ErrorMessage);
        }

        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;

            if (_debugLogging)
                Debug.Log("[IronSourceInitializer] Disposed");
        }
    }
}