// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Contrib.MassTransit.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    using global::MassTransit;
    using global::MassTransit.ExtensionsDependencyInjectionIntegration;
    using global::MassTransit.MultiBus;

    using Microsoft.Extensions.DependencyInjection;

    using StoneAssemblies.Contrib.MassTransit.Services;
    using StoneAssemblies.Contrib.MassTransit.Services.Interfaces;

    /// <summary>
    ///     The service collection extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     The dynamic buses assemblies.
        /// </summary>
        private static readonly ConcurrentDictionary<IServiceCollection, DynamicBusesAssembly> DynamicBusesAssemblies =
            new ConcurrentDictionary<IServiceCollection, DynamicBusesAssembly>();

        /// <summary>
        ///     The add bus selector.
        /// </summary>
        /// <param name="serviceCollection">
        ///     The service collection.
        /// </param>
        /// <param name="predicate">
        ///     The predicate.
        /// </param>
        public static void AddBusSelector<TMessage>(
            this IServiceCollection serviceCollection, Func<IBus, TMessage, Task<bool>> predicate = null)
            where TMessage : class
        {
            var dynamicBusesAssembly = DynamicBusesAssemblies.GetOrAdd(serviceCollection, sc => new DynamicBusesAssembly());
            if (dynamicBusesAssembly.BusTypes.Values.Count > 0)
            {
                foreach (var busTypesValue in dynamicBusesAssembly.BusTypes.Values)
                {
                    var serviceDescriptor = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == busTypesValue);
                    if (serviceDescriptor?.ImplementationFactory != null)
                    {
                        serviceCollection.AddSingleton(typeof(IBus), serviceDescriptor.ImplementationFactory);
                    }
                }

                if (predicate != null)
                {
                    serviceCollection.AddSingleton<IBusSelector<TMessage>, PredicateBasedBusSelector<TMessage>>();
                    serviceCollection.AddSingleton<IBusSelectorPredicate<TMessage>>(new BusSelectorPredicate<TMessage>(predicate));
                }
            }
            else
            {
                serviceCollection.AddSingleton<IBusSelector<TMessage>, DefaultBusSelector<TMessage>>();
            }

            serviceCollection.AddSingleton<IBusSelector>(provider => provider.GetService<IBusSelector<TMessage>>());
        }

        /// <summary>
        ///     The add mass transit.
        /// </summary>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="typeName">
        ///     The type name.
        /// </param>
        /// <param name="configure">
        ///     The configure.
        /// </param>
        /// <returns>
        ///     The <see cref="IServiceCollection" />.
        /// </returns>
        public static IServiceCollection AddMassTransit(
            this IServiceCollection collection, string typeName, Action<IServiceCollectionBusConfigurator> configure = null)
        {
            var dynamicBusesAssembly = DynamicBusesAssemblies.GetOrAdd(collection, sc => new DynamicBusesAssembly());

            dynamicBusesAssembly.Configures[typeName] = configure;

            var typeBuilder = dynamicBusesAssembly.ModuleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Interface);
            typeBuilder.AddInterfaceImplementation(typeof(IBus));
            var dynamicType = typeBuilder.CreateType();
            var addMassTransitMethod = typeof(DependencyInjectionMultiBusRegistrationExtensions).GetMethods().FirstOrDefault(
                info => info.Name == nameof(DependencyInjectionMultiBusRegistrationExtensions.AddMassTransit)
                        && info.ContainsGenericParameters && info.GetGenericArguments().Length == 1);

            if (dynamicType != null && addMassTransitMethod != null)
            {
                dynamicBusesAssembly.BusTypes[typeName] = dynamicType;

                var makeGenericMethod = addMassTransitMethod.MakeGenericMethod(dynamicType);
                var makeGenericType =
                    typeof(global::MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus.IServiceCollectionBusConfigurator<>)
                        .MakeGenericType(dynamicType);
                var delegateType = typeof(Action<>).MakeGenericType(makeGenericType);
                var forwardMethod = typeof(ServiceCollectionExtensions).GetMethod(
                    nameof(Forward),
                    BindingFlags.Static | BindingFlags.NonPublic);

                if (forwardMethod != null)
                {
                    var genericMethod = forwardMethod.MakeGenericMethod(makeGenericType);
                    var @delegate = Delegate.CreateDelegate(delegateType, genericMethod);
                    makeGenericMethod.Invoke(
                        typeof(DependencyInjectionMultiBusRegistrationExtensions),
                        new object[]
                            {
                                collection, @delegate
                            });
                }
            }

            return collection;
        }

        /// <summary>
        ///     The forward.
        /// </summary>
        /// <param name="configure">
        ///     The configure.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        private static void Forward<T>(T configure)
        {
            if (configure is IServiceCollectionBusConfigurator serviceCollectionBusConfigurator)
            {
                var genericArguments = configure.GetType().GetGenericArguments();
                var dynamicBusesAssembly = DynamicBusesAssemblies[serviceCollectionBusConfigurator.Collection];
                var action = dynamicBusesAssembly.Configures[genericArguments[0].Name];
                action?.Invoke(serviceCollectionBusConfigurator);
            }
        }

        /// <summary>
        ///     The dynamic buses assembly.
        /// </summary>
        private class DynamicBusesAssembly
        {
            /// <summary>
            ///     The stone assemblies dynamic buses.
            /// </summary>
            private const string StoneAssembliesDynamicBuses = "StoneAssemblies.DynamicBuses";

            /// <summary>
            ///     The count.
            /// </summary>
            private static int Count;

            /// <summary>
            ///     Initializes a new instance of the <see cref="DynamicBusesAssembly" /> class.
            /// </summary>
            public DynamicBusesAssembly()
            {
                var assemblyName = new AssemblyName(StoneAssembliesDynamicBuses);
                this.AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                this.ModuleBuilder = this.AssemblyBuilder.DefineDynamicModule(assemblyName.Name + $"{Count++}.dll");
            }

            /// <summary>
            ///     Gets the bus types.
            /// </summary>
            public Dictionary<string, Type> BusTypes { get; } = new Dictionary<string, Type>();

            /// <summary>
            ///     Gets the configures.
            /// </summary>
            public Dictionary<string, Action<IServiceCollectionBusConfigurator>> Configures { get; } =
                new Dictionary<string, Action<IServiceCollectionBusConfigurator>>();

            /// <summary>
            ///     Gets the module builder.
            /// </summary>
            public ModuleBuilder ModuleBuilder { get; }

            /// <summary>
            ///     Gets the assembly builder.
            /// </summary>
            private AssemblyBuilder AssemblyBuilder { get; }
        }
    }
}