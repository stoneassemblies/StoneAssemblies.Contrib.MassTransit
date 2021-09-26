// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionBusConfiguratorExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Contrib.MassTransit.Services.Extensions
{
    using System;

    using global::MassTransit;
    using global::MassTransit.ExtensionsDependencyInjectionIntegration;

    using StoneAssemblies.Contrib.MassTransit.Extensions;

    /// <summary>
    ///     The service collection bus configurator extensions.
    /// </summary>
    public static class ServiceCollectionBusConfiguratorExtensions
    {
        /// <summary>
        ///     Adds request client for a message.
        /// </summary>
        /// <param name="this">
        ///     The instance.
        /// </param>
        /// <param name="timeout">
        ///     The timeout.
        /// </param>
        /// <typeparam name="TMessage">
        ///     The message type
        /// </typeparam>
        public static void AddDefaultRequestClient<TMessage>(this IServiceCollectionBusConfigurator @this, RequestTimeout timeout = default)
            where TMessage : class
        {
            @this.AddRequestClient<TMessage>(new Uri($"queue:{typeof(TMessage).GetFlatName()}"), timeout);
        }
    }
}