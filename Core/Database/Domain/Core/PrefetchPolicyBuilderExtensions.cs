// <copyright file="PrefetchPolicyBuilderExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>


namespace Allors.Database.Domain
{
    using System.Linq;
    using Meta;

    public static partial class PrefetchPolicyBuilderExtensions
    {
        static PrefetchPolicyBuilderExtensions()
        {
        }

        public static void WithWorkspaceRules(this PrefetchPolicyBuilder @this, IClass @class)
        {
            // TODO: Cache
            foreach (var roleType in @class.DatabaseRoleTypes.Where(v => v.RelationType.WorkspaceNames.Length > 0))
            {
                _ = @this.WithRule(roleType);
            }
        }

        public static void WithSecurityRules(this PrefetchPolicyBuilder @this, Class @class, MetaPopulation m)
        {
            // TODO: Cache
            var AccessControlPrefetchPolicy = new PrefetchPolicyBuilder()
                .WithRule(m.AccessControl.UniqueId)
                .Build();

            var SecurityTokenPrefetchPolicy = new PrefetchPolicyBuilder()
                .WithRule(m.SecurityToken.AccessControls, AccessControlPrefetchPolicy)
                .Build();

            if (@class.DelegatedAccessRoleTypes != null)
            {
                var builder = new PrefetchPolicyBuilder()
                    .WithRule(m.Object.SecurityTokens, SecurityTokenPrefetchPolicy)
                    .WithRule(m.Object.DeniedPermissions)
                    .Build();

                var delegatedAccessRoleTypes = @class.DelegatedAccessRoleTypes;
                foreach (var delegatedAccessRoleType in delegatedAccessRoleTypes)
                {
                    _ = @this.WithRule(delegatedAccessRoleType, builder);
                }
            }

            _ = @this.WithRule(m.Object.SecurityTokens, SecurityTokenPrefetchPolicy);
            _ = @this.WithRule(m.Object.DeniedPermissions);
        }
    }
}
