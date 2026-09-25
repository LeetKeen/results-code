namespace LeetKeen.Results;

#pragma warning disable CA1034 // Nested types should not be visible

/// <summary>
/// Provides standard data keys for error data dictionaries.
/// </summary>
public static class StandardDataKeys
{
    /// <summary>
    /// Key for the name of a feature related to an error.
    /// </summary>
    public const string FeatureName = "FeatureName";


    /// <summary>
    /// Key for the name of an action related to an error.
    /// </summary>
    public const string ActionName = "ActionName";


    /// <summary>
    /// Provides standard data keys for business-related errors.
    /// </summary>
    public static class Business
    {
        /// <summary>
        /// Key for the kind of an entity related to a business error.
        /// </summary>
        public const string EntityKind = "EntityKind";


        /// <summary>
        /// Key for the identifier of an entity related to a business error.
        /// </summary>
        public const string EntityId = "EntityId";
    }


    /// <summary>
    /// Provides standard data keys for repository-related errors.
    /// </summary>
    public static class Repository
    {
        /// <summary>
        /// Key for the kind of a repository related to a repository error.
        /// </summary>
        public const string RepositoryKind = "RepositoryKind";
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
