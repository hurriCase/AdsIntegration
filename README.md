# AdsIntegration Package

A Unity integration package that provides a clean, flexible and type-safe way to implement and manage mobile advertisements using IronSource SDK.

## Features

- Easy integration with IronSource SDK
- Type-safe placement management through enums
- Configuration through ScriptableObject
- Support for rewarded and interstitial ads
- Editor tools for configuration management
- Flexible builder pattern for service initialization

## Setup

1. Install the IronSource SDK in your Unity project
2. Open the Ads Integration Settings window:
   ```
   Tools > Ads Integration Settings
   ```
3. Configure your IronSource credentials and ad settings
4. Create an enum with placement attributes to define your ad placements:
   ```csharp
   public enum AdPlacements
   {
       [Placement("Coins", "Rewarded coins for watching an ad")]
       WatchForCoins,
       
       [Placement("Gem", "Get a gem for watching an ad")]
       WatchForGem,
       
       [Placement("ExtraLife", "Get an extra life")]
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
- Configure via enum attributes or manually in the settings window
- Each placement has a name, reward type, and optional description

## Usage Example

### Creating Placement Enum
```csharp
public enum GameAdPlacements
{
    [Placement("Coins", "Get 50 coins for watching an ad")]
    WatchForCoins,
    
    [Placement("DoubleReward", "Double your mission rewards")]
    DoubleReward,
    
    [Placement("SkipLevel", "Skip this level")]
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

## Architecture

### Core Components
- **IAdService**: Main interface for showing ads
- **AdServiceConfig**: ScriptableObject for configuration
- **PlacementAttribute**: Attribute for marking enum values as placements
- **AdPlacementEditorUtility**: Editor tools for placement management

### Editor Tools
- **AdSettingsWindow**: Editor window for configuring the ad service
- **PlacementDefinitionDrawer**: Custom property drawer for placements

### Implementation Classes
- **IronSourceAdService**: IronSource implementation of IAdService
- **IronSourceInitializer**: Handles SDK initialization
- **IronSourceRewardedAdService**: Manages rewarded ads
- **IronSourceInterstitialAdService**: Manages interstitial ads

## Benefits of Enum-Based Placements

- Type safety and compile-time checking
- IntelliSense support in your IDE
- Centralized placement definitions
- Documentation through attributes

## Notes

- Always use the builder pattern to create the ad service
- Ad placements are managed through enums with the `[Placement]` attribute
- Use the editor to manage your configurations
- Test ads using the test mode before deploying to production