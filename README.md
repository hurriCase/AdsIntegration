# AdsIntegration Package

A Unity integration package that provides a clean, flexible and type-safe way to implement and manage mobile advertisements using IronSource SDK.

## Features

- Easy integration with IronSource SDK
- Type-safe placement management through enums
- Configuration through ScriptableObject
- Support for rewarded and interstitial ads
- Editor tools for configuration management
- Flexible builder pattern for service initialization
- Ad impression tracking for analytics
- Auto-reload ads after display

## Setup

1. Install the IronSource SDK in your Unity project
2. Open the Ads Integration Settings window:
   ```
   Tools > Ads Integration Settings
   ```
3. Configure your IronSource credentials and ad settings
4. Create an enum marked with the Placement attribute to define your ad placements:
   ```csharp
   [Placement]
   public enum AdPlacements
   {
       WatchForCoins,
       WatchForGem,
       ExtraLife
   }
   ```
5. In the Ads Integration Settings window, select your enum type and apply it
6. Initialize and use the ad service in your game code

## Configuration Properties

### IronSource Settings
- **App Key**: Your IronSource app key
- **Rewarded Ad Unit ID**: Unit ID for rewarded ads
- **Interstitial Ad Unit ID**: Unit ID for interstitial ads

### Ad Settings
- **Time Between Interstitials**: Minimum time (in seconds) between interstitial ads
- **Max Interstitial Load Attempts**: How many times to retry loading interstitial ads
- **Max Rewarded Load Attempts**: How many times to retry loading rewarded ads
- **Retry Load Delay**: Time (in seconds) to wait before retrying to load ads

### Placement Configuration
- Configure via enum attributes in your code
- Each placement is defined by an enum value

## Usage Example

### Creating Placement Enum
```csharp
using AdsIntegration.Runtime.Config;

[Placement]
public enum GameAdPlacements
{
    WatchForCoins,
    DoubleReward,
    SkipLevel
}
```

### Initializing the Ad Service
```csharp
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Builder;

public class AdManager : MonoBehaviour
{
    private IAdService _adService;
    
    private void Awake()
    {
        // Create and configure the ad service
        _adService = AdServiceFactory.CreateIronSourceBuilder()
            .WithDebugLogging()
            .OnInitialized(() => Debug.Log("Ads initialized!"))
            .OnInitializationFailed(error => Debug.LogError($"Ad init failed: {error}"))
            .OnRewardedAdAvailabilityChanged(available => UpdateRewardedAdButton(available))
            .Build();
            
        // Initialize the service
        _adService.Initialize();
    }
    
    // Show a rewarded ad using the enum placement
    public void ShowRewardedAd()
    {
        _adService.ShowRewardedAd(GameAdPlacements.WatchForCoins, () => 
        {
            // Reward the player
            GiveCoinsToPlayer(50);
        });
    }
    
    // Try to show an interstitial ad
    public void TryShowInterstitial()
    {
        bool shown = _adService.TryShowInterstitial();
        Debug.Log($"Interstitial shown: {shown}");
    }
    
    private void OnDestroy()
    {
        // Clean up
        if (_adService is IDisposable disposable)
            disposable.Dispose();
    }
}
```

### Adding Ad Impression Tracking
```csharp
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Builder;

public class AnalyticsImpressionTracker : IAdImpressionTracker
{
    public void TrackAdImpression(IronSourceImpressionData impressionData)
    {
        // Send impression data to your analytics service
        Debug.Log($"Ad impression: {impressionData}");
        
        // Example of sending to analytics:
        // AnalyticsService.TrackRevenue(
        //     impressionData.Revenue,
        //     impressionData.AdNetwork,
        //     impressionData.AdUnitName,
        //     impressionData.PlacementName);
    }
    
    public void Dispose()
    {
        // Clean up any resources if needed
    }
}

// When initializing the ad service:
var tracker = new AnalyticsImpressionTracker();
_adService = AdServiceFactory.CreateIronSourceBuilder()
    .WithDebugLogging()
    .WithAnalyticsService(tracker)
    .Build();
```

## Architecture

### Core Components
- **IAdService**: Main interface for showing ads
- **IAdServiceBuilder**: Builder interface for creating and configuring ad services
- **IAdImpressionTracker**: Interface for tracking ad impressions
- **AdServiceConfig**: ScriptableObject for configuration
- **PlacementAttribute**: Attribute for marking enum values as placements
- **PlacementExtensions**: Utilities for working with placement enums

### Implementation Classes
- **IronSourceAdService**: IronSource implementation of IAdService
- **IronSourceInitializer**: Handles SDK initialization
- **IronSourceRewardedAdService**: Manages rewarded ads
- **IronSourceInterstitialAdService**: Manages interstitial ads
- **IronSourceAdServiceBuilder**: Builder for creating IronSource ad services

## Key Features

### Automatic Ad Reloading
The system automatically attempts to reload ads in several situations:
- After an ad is closed
- When changing scenes
- When the app regains focus
- After failed load attempts (with configurable retry limits)

### Interstitial Cooldown
Interstitial ads have a configurable cooldown period to prevent overwhelming users. The `TryShowInterstitial()` method automatically respects this cooldown.

### Test Mode
Enable test mode during development to access the IronSource test suite:
```csharp
_adService = AdServiceFactory.CreateIronSourceBuilder()
    .WithTestMode()
    .Build();
```

### Proper Resource Cleanup
All services implement `IDisposable` to ensure proper cleanup of resources, event handlers, and SDK connections.

## Best Practices

- Always use the builder pattern to create the ad service
- Define your ad placements using an enum marked with the `[Placement]` attribute
- Track ad impressions for revenue analytics
- Use debug logging during development
- Test with test mode before deploying to production
- Properly dispose of the ad service when it's no longer needed
- Show interstitial ads at natural break points in your game
- Make rewarded ads provide meaningful value to players