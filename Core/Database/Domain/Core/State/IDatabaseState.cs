// <copyright file="IDatabaseState.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors
{
    using Meta;
    using State;

    public partial interface IDatabaseState : IDatabaseStateLifecycle
    {
        IDatabase Database { get; }

        MetaPopulation MetaPopulation { get; }

        M M { get; }

        IWorkspaceMetaCache WorkspaceMetaCache { get; set; }

        IPrefetchPolicyCache PrefetchPolicyCache { get; }

        IPermissionsCache PermissionsCache { get; }

        IEffectivePermissionCache EffectivePermissionCache { get; }

        IWorkspaceEffectivePermissionCache WorkspaceEffectivePermissionCache { get; }
        
        IDerivationFactory DerivationFactory { get; }

        IPreparedExtents PreparedExtents { get; }

        IPreparedFetches PreparedFetches { get; }

        IMetaCache MetaCache { get; }

        IPasswordHasher PasswordHasher { get; }

        ICaches Caches { get; }

        ITime Time { get; }

        ITreeCache TreeCache { get; }
    }
}