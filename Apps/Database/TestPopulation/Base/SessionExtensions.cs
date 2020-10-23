// <copyright file="SessionExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors
{
    using Bogus;

    public static class SessionExtensions
    {
        public static Faker Faker(this ISession @this) => ((dynamic)@this.Database.State()).Faker;
    }
}