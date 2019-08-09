//-------------------------------------------------------------------------------------------------
// <copyright file="PrefetchPolicyBuilderExtensions.cs" company="Allors bvba">
// Copyright 2002-2017 Allors bvba.
//
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
//
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
//
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>Defines the ISessionExtension type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Allors
{
    using Meta;

    public static partial class PrefetchPolicyBuilderExtensions
    {
        private static readonly PrefetchPolicy AccessControlPrefetchPolicy;

        private static readonly PrefetchPolicy SecurityTokenPrefetchPolicy;

        static PrefetchPolicyBuilderExtensions()
        {
            AccessControlPrefetchPolicy = new PrefetchPolicyBuilder()
                .WithRule(MetaAccessControl.Instance.CacheId.RoleType)
                .Build();

            SecurityTokenPrefetchPolicy = new PrefetchPolicyBuilder()
                .WithRule(MetaSecurityToken.Instance.AccessControls, AccessControlPrefetchPolicy)
                .Build();
        }

        public static void WithWorkspaceRules(this PrefetchPolicyBuilder @this, Class @class)
        {
            foreach (var roleType in @class.WorkspaceRoleTypes)
            {
                @this.WithRule(roleType);
            }
        }

        public static void WithSecurityRules(this PrefetchPolicyBuilder @this, Class @class)
        {
            if (@class.DelegatedAccessRoleTypes != null)
            {
                var builder = new PrefetchPolicyBuilder()
                    .WithRule(MetaObject.Instance.SecurityTokens, SecurityTokenPrefetchPolicy)
                    .WithRule(MetaObject.Instance.DeniedPermissions)
                    .Build();

                var delegatedAccessRoleTypes = @class.DelegatedAccessRoleTypes;
                foreach (var delegatedAccessRoleType in delegatedAccessRoleTypes)
                {
                    @this.WithRule(delegatedAccessRoleType, builder);
                }
            }

            @this.WithRule(MetaObject.Instance.SecurityTokens, SecurityTokenPrefetchPolicy);
            @this.WithRule(MetaObject.Instance.DeniedPermissions);
        }
    }
}