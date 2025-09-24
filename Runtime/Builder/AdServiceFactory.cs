using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;

namespace AdsIntegration.Runtime.Builder
{
    /// <summary>
    /// Factory class for creating ad service instances.
    /// </summary>
    /// <remarks>
    /// This class provides the entry point for creating and configuring ad services in your application.
    /// </remarks>
    [UsedImplicitly]
    public static class AdServiceFactory
    {
        /// <summary>
        /// Creates a new IronSource ad service builder.
        /// </summary>
        /// <returns>A builder instance for configuring and creating an IronSource ad service.</returns>
        /// <remarks>
        /// Use the returned builder to configure the ad service with your desired settings and callbacks,
        /// then call Build() to create the service instance.
        /// </remarks>
        [UsedImplicitly]
        public static IAdServiceBuilder CreateIronSourceBuilder() => new IronSourceAdServiceBuilder();
    }
}