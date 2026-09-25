using System;

namespace LeetKeen.Results;

#pragma warning disable CA1034 // Nested types should not be visible

/// <summary>
/// Provides standard error codes for transport and business logic errors.
/// </summary>
public static class StandardErrorCodes
{
    /// <summary>
    /// Used when no other error code may be applied.
    /// </summary>
    public const int GeneralError = -1;

    /// <summary>
    /// An error occurred on a transport layer. 
    /// Not related to business processes. 
    /// Never occurs when using in-process transport.
    /// </summary>
    public const int TransportError = -2;

    /// <summary>
    /// Data for request is malformed in client.
    /// </summary>
    public const int MalformedRequest = -3;

    /// <summary>
    /// Unrecoverable error in internal systems of the called side.
    /// </summary>
    public const int RemoteInternalError = -4;

    /// <summary>
    /// Request cannot be preformed because remote side does not currently 
    /// provide the related future.
    /// </summary>
    public const int FeatureUnavailable = -5;

    /// <summary>
    /// Remote point requires an authenticated user.
    /// </summary>
    public const int NoIdentity = -6;

    /// <summary>
    /// Remote point refuses to act because the provided identity 
    /// is not allowed to perform an action or access data.
    /// </summary>
    public const int NoPermission = -7;

    /// <summary>
    /// A generic error for any business logic errors. Reasons of an error 
    /// should be inspected for specific details.
    /// </summary>
    public const int BusinessError = -8;


    /// <summary>
    /// An error occurred in the repository layer.
    /// See internal reasons for details.
    /// </summary>
    [Obsolete("Use Repository.ErrorReadingFromRepository " +
        "or Repository.ErrorWritingToRepository instead " +
        "or other Repository... errors.")]
    public const int RepositoryError = -9;


    /// <summary>
    /// Determines if the code is a standard error code.
    /// </summary>
    /// <param name="code">The error code to check.</param>
    /// <returns>True if the code is a standard error code; otherwise, false.</returns>
    public static bool IsStandardCode(int code)
    {
        return code == GeneralError
            || code == TransportError
            || code == MalformedRequest
            || code == RemoteInternalError
            || code == FeatureUnavailable
            || code == NoIdentity
            || code == NoPermission
            || code == BusinessError;
    }


    /// <summary>
    /// Contains standard business error codes.
    /// </summary>
    public static class Business
    {
        private const int BASE = -1000;

        /// <summary>
        /// Client requests an entity by ID, but it does not exist.
        /// </summary>
        public const int EntityNotFound = BASE - 1;

        /// <summary>
        /// Authentication failed due to invalid credentials.
        /// </summary>
        public const int InvalidAuthCredentials = BASE - 2;

        /// <summary>
        /// Remote side cannot create an entity as it already exists.
        /// </summary>
        public const int CantCreateExists = BASE - 3;


        /// <summary>
        /// Determines if the code is a standard business error code.
        /// </summary>
        /// <param name="code">The error code to check.</param>
        /// <returns>True if the code is a standard business error code; otherwise, false.</returns>
        public static bool IsStandardBusinessCode(int code)
        {
            return code == EntityNotFound
                || code == InvalidAuthCredentials
                || code == CantCreateExists;
        }
    }


    /// <summary>
    /// Contains standard repository error codes.
    /// </summary>
    public static class Repository
    {
        private const int BASE = -2000;


        /// <summary>
        /// An error occurred while trying to read data from the repository.
        /// </summary>
        public const int FailedToRead = BASE - 1;


        /// <summary>
        /// An error occurred while trying to write data to the repository.
        /// </summary>
        public const int FailedToWrite = BASE - 2;


        /// <summary>
        /// Determines whether the specified code represents a standard 
        /// repository error code.
        /// </summary>
        /// <param name="code">The integer code to evaluate.</param>
        /// <returns>
        /// <see langword="true"/> if the code is a standard repository error 
        /// code; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsStandardRepositoryCode(int code)
        {
            return code == FailedToRead
                || code == FailedToWrite;
        }
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
