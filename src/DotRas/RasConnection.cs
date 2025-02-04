﻿using System;
using System.Collections.Generic;
using System.Threading;
using DotRas.Internal;
using DotRas.Internal.Abstractions.Services;

namespace DotRas
{
    /// <summary>
    /// Represents a remote access connection.
    /// </summary>
    public class RasConnection : IRasConnection, IEquatable<RasConnection>
    {
        #region Fields and Properties

        private readonly IRasGetConnectStatus statusService;
        private readonly IRasHangUp hangUpService;
        private readonly IRasGetConnectionStatistics connectionStatisticsService;
        private readonly IRasClearConnectionStatistics clearConnectionStatisticsService;

        /// <summary>
        /// Gets the handle of the connection.
        /// </summary>
        public virtual IntPtr Handle { get; }

        /// <summary>
        /// Gets the device through which the connection has been established.
        /// </summary>
        public virtual RasDevice Device { get; }

        /// <summary>
        /// Gets the name of the phone book entry used to establish the remote access connection.
        /// </summary>
        public virtual string EntryName { get; }

        /// <summary>
        /// Gets the full path (including filename) to the phone book containing the entry for this connection.
        /// </summary>
        public virtual string PhoneBookPath { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> that represents the phone book entry.
        /// </summary>
        public virtual Guid EntryId { get; }

        /// <summary>
        /// Gets the connection options.
        /// </summary>
        public virtual RasConnectionOptions Options { get; }

        /// <summary>
        /// Gets the <see cref="Luid"/> that represents the logon session in which the connection was established.
        /// </summary>
        public virtual Luid SessionId { get; }

        /// <summary>
        /// Gets the correlation id.
        /// </summary>
        public virtual Guid CorrelationId { get; }

        #endregion

        internal RasConnection(IntPtr handle, RasDevice device, string entryName, string phoneBookPath, Guid entryId, RasConnectionOptions options, Luid sessionId, Guid correlationId, IRasGetConnectStatus statusService, IRasGetConnectionStatistics connectionStatisticsService, IRasHangUp hangUpService, IRasClearConnectionStatistics clearConnectionStatisticsService)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(handle));
            }
            else if (string.IsNullOrWhiteSpace(entryName))
            {
                throw new ArgumentNullException(nameof(entryName));
            }
            else if (string.IsNullOrWhiteSpace(phoneBookPath))
            {
                throw new ArgumentNullException(nameof(phoneBookPath));
            }

            EntryName = entryName;
            PhoneBookPath = phoneBookPath;
            Handle = handle;
            Device = device ?? throw new ArgumentNullException(nameof(device));
            EntryId = entryId;
            Options = options ?? throw new ArgumentNullException(nameof(options));
            SessionId = sessionId;
            CorrelationId = correlationId;

            this.statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            this.connectionStatisticsService = connectionStatisticsService ?? throw new ArgumentNullException(nameof(connectionStatisticsService));
            this.hangUpService = hangUpService ?? throw new ArgumentNullException(nameof(hangUpService));
            this.clearConnectionStatisticsService = clearConnectionStatisticsService ?? throw new ArgumentNullException(nameof(clearConnectionStatisticsService));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RasConnection"/> class.
        /// </summary>
        protected RasConnection()
        {
        }

        /// <summary>
        /// Enumerates the connections.
        /// </summary>
        /// <returns>An enumerable used to iterate through the connections.</returns>
        public static IEnumerable<RasConnection> EnumerateConnections()
        {
            return ServiceLocator.Default.GetRequiredService<IRasEnumConnections>()
                .EnumerateConnections();
        }

        /// <summary>
        /// Clears the accumulated statistics for the connection.
        /// </summary>
        /// <exception cref="RasException">Thrown if the connection has been terminated.</exception>
        public virtual void ClearStatistics()
        {
            clearConnectionStatisticsService.ClearConnectionStatistics(this);
        }

        /// <summary>
        /// Retrieves accumulated statistics for the connection.
        /// </summary>
        /// <exception cref="RasException">Thrown if the connection has been terminated.</exception>
        public virtual RasConnectionStatistics GetStatistics()
        {
            return connectionStatisticsService.GetConnectionStatistics(this);
        }

        /// <summary>
        /// Retrieves the connection status.
        /// </summary>
        /// <exception cref="RasException">Thrown if the connection has been terminated.</exception>
        public virtual RasConnectionStatus GetStatus()
        {
            return statusService.GetConnectionStatus(this);
        }

        /// <summary>
        /// Disconnects the remote access connection.
        /// </summary>
        public virtual void Disconnect()
        {
            Disconnect(CancellationToken.None);
        }

        /// <summary>
        /// Disconnects the remote access connection.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        public virtual void Disconnect(CancellationToken cancellationToken)
        {
            Disconnect(true, cancellationToken);
        }

        /// <summary>
        /// Disconnects the remote access connection.
        /// </summary>
        /// <param name="closeAllReferences">true to close all referenced connections to the handle, otherwise false to only close this reference.</param>
        /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
        /// <exception cref="OperationCanceledException">The operation has been cancelled.</exception>
        public virtual void Disconnect(bool closeAllReferences, CancellationToken cancellationToken)
        {
            hangUpService.HangUp(this, closeAllReferences, cancellationToken);
        }

        /// <summary>
        /// Evaluates the equality of two instances.
        /// </summary>
        /// <param name="objA">The first instance.</param>
        /// <param name="objB">The second instance.</param>
        /// <returns>A value indicating whether the two objects are not equal.</returns>
        public static bool operator ==(RasConnection objA, RasConnection objB)
        {
#pragma warning disable IDE0041 // Use 'is null' check
            if (ReferenceEquals(objA, null) && ReferenceEquals(objB, null))
            {
                return true;
            }
            else if (ReferenceEquals(objA, null) || ReferenceEquals(objB, null))
            {
                return false;   
            }
#pragma warning restore IDE0041 // Use 'is null' check

            return objA.Equals(objB);
        }

        /// <summary>
        /// Evaluates the inequality of two instances.
        /// </summary>
        /// <param name="objA">The first instance.</param>
        /// <param name="objB">The second instance.</param>
        /// <returns>A value indicating whether the two objects are not equal.</returns>

        public static bool operator !=(RasConnection objA, RasConnection objB)
        {
            return !(objA == objB);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = obj as RasConnection;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        /// <inheritdoc />
        public virtual bool Equals(RasConnection other)
        {
            if (other == null)
            {
                return false;
            }

            return Handle == other.Handle;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        }
    }
}