﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBusSelector.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Contrib.MassTransit.Services
{
    using System.Collections.Generic;

    using global::MassTransit;

    using StoneAssemblies.Contrib.MassTransit.Services.Interfaces;

    /// <summary>
    ///     The default bus selector.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class DefaultBusSelector<TMessage> : IBusSelector<TMessage>
        where TMessage : class
    {
        /// <summary>
        ///     The bus.
        /// </summary>
        private readonly IBus bus;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultBusSelector{TMessage}" /> class.
        /// </summary>
        /// <param name="bus">
        ///     The bus.
        /// </param>
        public DefaultBusSelector(IBus bus)
        {
            this.bus = bus;
        }

        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
        public async IAsyncEnumerable<IClientFactory> SelectClientFactories(TMessage message)
        {
            yield return this.bus.CreateClientFactory();
        }

        /// <summary>
        ///     The select client factories.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        /// <returns>
        ///     The <see cref="IAsyncEnumerable{IClientFactory}" />.
        /// </returns>
        public async IAsyncEnumerable<IClientFactory> SelectClientFactories(object message)
        {
            yield return this.bus.CreateClientFactory();
        }
    }
}