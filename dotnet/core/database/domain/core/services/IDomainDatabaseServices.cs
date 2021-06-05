// <copyright file="IDomainDatabaseServices.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Database.Data;
    using Meta;

    public partial interface IDomainDatabaseServices : IDatabaseServices
    {
        IDatabase Database { get; }

        MetaPopulation M { get; }

        IPrefetchPolicyCache PrefetchPolicyCache { get; }

        IPermissionsCache PermissionsCache { get; }

        IAccessControlCache AccessControlCache { get; }

        IDerivationFactory DerivationFactory { get; }

        IPreparedExtents PreparedExtents { get; }

        IPreparedSelects PreparedSelects { get; }

        IMetaCache MetaCache { get; }

        IPasswordHasher PasswordHasher { get; }

        ICaches Caches { get; }

        ITime Time { get; }

        ITreeCache TreeCache { get; }

        IClassById ClassById { get; }

        IVersionedIdByStrategy VersionedIdByStrategy { get; }
    }
}