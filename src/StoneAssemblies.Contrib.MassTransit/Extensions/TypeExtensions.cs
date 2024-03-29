﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Stone Assemblies">
// Copyright © 2021 - 2021 Stone Assemblies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StoneAssemblies.Contrib.MassTransit.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    ///     The type extensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Gets a type flat name.
        /// </summary>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The flat name.
        /// </returns>
        public static string GetFlatName(this Type type)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(type.Name.Split('`')[0]);
            foreach (var genericArgument in type.GetGenericArguments())
            {
                builder.Append($"-{genericArgument.GetFlatName()}");
            }

            return builder.ToString();
        }
    }
}
