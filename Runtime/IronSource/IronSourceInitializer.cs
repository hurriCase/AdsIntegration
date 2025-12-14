using AdsIntegration.Runtime.Base;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.IronSource
{
    internal sealed class IronSourceInitializer : IAdInitializer
    {
        public bool IsInitialized { get; private set; }

        private readonly string _appKey;

        public Observable<Unit> OnInitializationCompleted => _initializationCompleted;
        private readonly Subject<Unit> _initializationCompleted = new();

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

            _initializationCompleted.OnNext(Unit.Default);
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;

            Logger.LogError($"[IronSourceInitializer::SdkInitializationFailedEvent] " +
                            $"SDK initialization failed: {levelPlayInitError.ErrorMessage}");
        }

        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;

            Logger.Log("[IronSourceInitializer::Dispose] Disposed");
        }
    }
}