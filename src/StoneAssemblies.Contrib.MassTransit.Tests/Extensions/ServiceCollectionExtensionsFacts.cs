// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsFacts.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Contrib.MassTransit.Tests.Extensions
{
    using System.Linq;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.Contrib.MassTransit.Extensions;

    using Xunit;

    /// <summary>
    ///     The service collection extensions facts.
    /// </summary>
    public class ServiceCollectionExtensionsFacts
    {
        /// <summary>
        ///     The the add mass transit method.
        /// </summary>
        public class The_AddMassTransit_Method
        {
            /// <summary>
            ///     The invokes the configuration action.
            /// </summary>
            [Fact]
            public void Invokes_The_Configuration_Action()
            {
                var invoked = false;
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddMassTransit("ThirdBus", configurator => { invoked = true; });
                Assert.True(invoked);
            }

            /// <summary>
            ///     The registers the generated bus type.
            /// </summary>
            [Fact]
            public void Registers_The_Generated_BusType()
            {
                const string BusTypeName = "SecondBus";

                var serviceCollection = new ServiceCollection();
                serviceCollection.AddMassTransit(BusTypeName);
                var serviceDescriptor = serviceCollection.FirstOrDefault(
                    descriptor => descriptor.ServiceType.GetGenericArguments().Length > 0
                                  && descriptor.ServiceType.GetGenericArguments()[0].Name == BusTypeName);

                Assert.NotNull(serviceDescriptor);
            }
        }
    }
}