using AdsIntegration.Runtime.Base;

namespace AdsIntegration.Runtime.Builder
{
    /// <summary>
    /// Factory for creating ad service instances
    /// </summary>
    public static class AdServiceFactory
    {
        /// <summary>
        /// Creates a builder for an IronSource ad service
        /// </summary>
        /// <returns>A builder for configuring and creating an IronSource ad service</returns>
        public static IAdServiceBuilder CreateIronSourceBuilder() => new IronSourceAdServiceBuilder();
    }
}