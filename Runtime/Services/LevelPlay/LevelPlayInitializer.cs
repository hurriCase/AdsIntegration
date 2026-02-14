using AdsIntegration.Runtime.Base;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Services.IronSource
{
    internal sealed class LevelPlayInitializer : IAdInitializer
    {
        public bool IsInitialized { get; private set; }

        private readonly string _appKey;

        public Observable<Unit> OnInitializationCompleted => _initializationCompleted;
        private readonly Subject<Unit> _initializationCompleted = new();

        public LevelPlayInitializer(string appKey)
        {
            _appKey = appKey;
        }

        public void Init()
        {
            Logger.Log($"[LevelPlayInitializer::Init] Initializing IronSource SDK with app key: {_appKey}");

            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

            LevelPlay.Init(_appKey);
        }

        private void SdkInitializationCompletedEvent(LevelPlayConfiguration levelPlayConfiguration)
        {
            IsInitialized = true;

            Logger.Log("[LevelPlayInitializer::SdkInitializationCompletedEvent] SDK initialized successfully");

            _initializationCompleted.OnNext(Unit.Default);
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;

            Logger.LogError($"[LevelPlayInitializer::SdkInitializationFailedEvent] " +
                            $"SDK initialization failed: {levelPlayInitError.ErrorMessage}");
        }

        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;

            Logger.Log("[LevelPlayInitializer::Dispose] Disposed");
        }
    }
}