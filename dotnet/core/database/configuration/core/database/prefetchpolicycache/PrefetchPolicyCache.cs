// <copyright file="IPreparedSelects.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Configuration
{
    using Database;
    using Domain;

    public partial class PrefetchPolicyCache : IPrefetchPolicyCache
    {
        public PrefetchPolicyCache(IDomainDatabaseServices domainDatabaseServices)
        {
            this.DomainDatabaseServices = domainDatabaseServices;

            var m = this.DomainDatabaseServices.M;

            this.PermissionsWithClass = new PrefetchPolicyBuilder()
                    .WithRule(m.Permission.ClassPointer)
                    .Build();
        }

        public IDomainDatabaseServices DomainDatabaseServices { get; }

        public PrefetchPolicy PermissionsWithClass { get; }
    }
}
