// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;
    using Derivations.Rules;

    public class AccessControlEffectivePermissionsRule : Rule
    {
        public AccessControlEffectivePermissionsRule(MetaPopulation m) : base(m, new Guid("1F897B84-EF92-4E94-8877-3501D56D426B")) =>
            this.Patterns = new Pattern[]
            {
                m.AccessControl.RolePattern(v=>v.Role),
                m.Role.RolePattern(v=>v.Permissions, v=>v.AccessControlsWhereRole),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var accessControl in matches.Cast<AccessControl>())
            {
                accessControl.EffectivePermissions = (accessControl.Role?.Permissions.ToArray());

                // Invalidate cache
                accessControl.DatabaseServices().Get<IAccessControlCache>().Clear(accessControl.Id);
            }
        }
    }
}
