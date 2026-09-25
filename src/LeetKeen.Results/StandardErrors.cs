namespace LeetKeen.Results;

#pragma warning disable CA1034 // Nested types should not be visible

/// <summary>
/// Provides factory methods for creating standard error objects representing 
/// common error conditions.
/// </summary>
/// <remarks>
/// Use the methods of this class to generate consistent error representations 
/// for common scenarios such as general failures or missing entities. 
/// This helps ensure uniform error handling across the application.
/// </remarks>
public static class StandardErrors
{
    /// <inheritdoc cref="StandardErrorCodes.GeneralError"/>.
    /// <param name="message">
    /// The error message. If null, it defaults 
    /// to &quot;A general error occurred.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a general error.
    /// </returns>
    public static IError GeneralError(string message)
    {
        return new Error(StandardErrorCodes.GeneralError,
            message ?? "A general error occurred.");
    }


    /// <inheritdoc cref="StandardErrorCodes.TransportError"/>.
    /// <param name="message">
    /// The error message. If null, it defaults 
    /// to &quot;A transport layer error occurred.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a transport error.
    /// </returns>
    public static IError TransportError(string message)
    {
        return new Error(StandardErrorCodes.TransportError,
            message ?? "A transport layer error occurred.");
    }


    /// <inheritdoc cref="StandardErrorCodes.MalformedRequest"/>.
    /// <param name="message">
    /// The error message. If null, it defaults
    /// to &quot;The request data is malformed.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a malformed request error.
    /// </returns>
    public static IError MalformedRequest(string message)
    {
        return new Error(StandardErrorCodes.MalformedRequest,
            message ?? "The request data is malformed.");
    }


    /// <inheritdoc cref="StandardErrorCodes.RemoteInternalError"/>.
    /// <param name="message">
    /// The error message. If null, it defaults
    /// to &quot;An internal error occurred on the remote side.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a remote internal error.
    /// </returns>
    public static IError RemoteInternalError(string message)
    {
        return new Error(StandardErrorCodes.RemoteInternalError,
            message ?? "An internal error occurred on the remote side.");
    }


    /// <inheritdoc cref="StandardErrorCodes.FeatureUnavailable"/>.
    /// <param name="featureName">
    /// The name of the unavailable feature.
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a feature unavailability error.
    /// </returns>
    public static IError FeatureUnavailable(
        string featureName)
    {
        return new Error(StandardErrorCodes.FeatureUnavailable,
            $"The feature {(featureName ?? string.Empty) + " "}is unavailable.")
            .WithData(StandardDataKeys.FeatureName, featureName ?? string.Empty);
    }


    /// <inheritdoc cref="StandardErrorCodes.NoIdentity"/>.
    /// <param name="message">
    /// The error message. If null, it defaults
    /// to &quot;No identity was provided for an authenticated request.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a no identity error.
    /// </returns>
    public static IError NoIdentity(
        string? message = null)
    {
        return new Error(StandardErrorCodes.NoIdentity,
            message ??
            "No identity was provided for an authenticated request.");
    }


    /// <inheritdoc cref="StandardErrorCodes.NoPermission"/>.
    /// <param name="actionName">
    /// The name of the action for which permission was denied.
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a no permission error.
    /// </returns>
    public static IError NoPermission(
        string actionName)
    {
        return new Error(StandardErrorCodes.NoPermission,
            "The provided identity does not have permission to perform the action.")
            .WithData(StandardDataKeys.ActionName, actionName ?? string.Empty);
    }


    /// <inheritdoc cref="StandardErrorCodes.BusinessError"/>.
    /// <param name="message">
    /// The error message. If null, it defaults
    /// to &quot;A business logic error occurred.&quot;
    /// </param>
    /// <returns>
    /// An <see cref="IError"/> representing a business logic error.
    /// </returns>
    public static IError BusinessError(
        string message)
    {
        return new Error(StandardErrorCodes.BusinessError,
            message ?? "A business logic error occurred.");
    }


    /// <summary>
    /// Provides factory methods for creating standard business-related 
    /// error objects.
    /// </summary>
    public static class Business
    {
        /// <inheritdoc cref="StandardErrorCodes.Business.EntityNotFound"/>.
        /// <param name="entityKind">
        /// The kind of the entity type that was not found.
        /// </param>
        /// <param name="entityId">
        /// The identifier of the entity that was not found.
        /// </param>
        /// <returns>
        /// An <see cref="IError"/> representing an entity not found error.
        /// </returns>
        public static IError EntityNotFound(
            string entityKind,
            object entityId)
        {
            var message = $"The entity '{entityKind ?? string.Empty}' was " +
                $"not found " + (entityId != null
                    ? $"with ID '{entityId}'."
                    : "(no ID provided).");

            return new Error(StandardErrorCodes.Business.EntityNotFound,
                message)
                .WithData(StandardDataKeys.Business.EntityKind,
                    entityKind ?? string.Empty)
                .WithData(StandardDataKeys.Business.EntityId,
                    entityId);
        }


        /// <inheritdoc cref="StandardErrorCodes.Business.InvalidAuthCredentials"/>.
        /// <returns>
        /// An <see cref="IError"/> representing an invalid authentication
        /// credentials error.
        /// </returns>
        public static IError InvalidAuthCredentials()
        {
            return new Error(StandardErrorCodes.Business.InvalidAuthCredentials,
                "Authentication failed due to invalid credentials.");
        }


        /// <inheritdoc cref="StandardErrorCodes.Business.CantCreateExists"/>.
        /// <param name="entityKind">
        /// The kind of the entity type that cannot be created.
        /// </param>
        /// <param name="entityId">
        /// The identifier of the entity that already exists.
        /// </param>
        /// <returns>
        /// An <see cref="IError"/> representing a cannot create
        /// as exists error.
        /// </returns>
        public static IError CantCreateExists(
            string entityKind,
            object entityId)
        {
            var message = $"Cannot create entity '{entityKind ?? string.Empty}' " +
                $"as it already exists " + (entityId != null
                    ? $"with ID '{entityId}'."
                    : "(no ID provided).");

            return new Error(
                StandardErrorCodes.Business.CantCreateExists, message)
                .WithData(StandardDataKeys.Business.EntityKind,
                    entityKind ?? string.Empty)
                .WithData(StandardDataKeys.Business.EntityId,
                    entityId);
        }
    }


    /// <summary>
    /// Provides factory methods for creating standard repository-related
    /// error objects.
    /// </summary>
    public static class Repository
    {
        /// <inheritdoc cref="StandardErrorCodes.Repository.FailedToRead"/>.
        /// <returns>
        /// An <see cref="IError"/> representing a repository read failure.
        /// </returns>
        public static IError FailedToRead(
            string repositoryKind = "")
        {
            return new Error(StandardErrorCodes.Repository.FailedToRead,
                $"An error occurred while reading from the " +
                $"{(string.IsNullOrEmpty(repositoryKind)
                    ? "repository"
                    : repositoryKind + " repository")}.")
                .WithData(StandardDataKeys.Repository.RepositoryKind,
                    repositoryKind ?? string.Empty);
        }


        /// <inheritdoc cref="StandardErrorCodes.Repository.FailedToWrite"/>.
        /// <returns>
        /// An <see cref="IError"/> representing a repository write failure.
        /// </returns>
        public static IError FailedToWrite(
            string repositoryKind = "")
        {
            return new Error(StandardErrorCodes.Repository.FailedToWrite,
                $"An error occurred while writing to the " +
                $"{(string.IsNullOrEmpty(repositoryKind)
                    ? "repository"
                    : repositoryKind + " repository")}.")
                .WithData(StandardDataKeys.Repository.RepositoryKind,
                    repositoryKind ?? string.Empty);
        }
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
