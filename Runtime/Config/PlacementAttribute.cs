using System;

namespace AdsIntegration.Runtime.Config
{
    /// <summary>
    /// Attribute used to mark an enum as containing ad placement definitions.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to enum types that represent collections of ad placements.
    /// Each enum value will be treated as a separate placement with its name used as the placement identifier.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class PlacementAttribute : Attribute { }
}