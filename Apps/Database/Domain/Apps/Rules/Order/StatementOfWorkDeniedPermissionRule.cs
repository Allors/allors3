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
    using Database.Derivations;

    public class StatementOfWorkDeniedPermissionRule : Rule
    {
        public StatementOfWorkDeniedPermissionRule(MetaPopulation m) : base(m, new Guid("374f5554-ea5f-4186-913b-0d24f06a02e5")) =>
            this.Patterns = new Pattern[]
        {
            m.StatementOfWork.RolePattern(v => v.TransitionalDeniedPermissions),
            m.StatementOfWork.RolePattern(v => v.Request),
        };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var transaction = cycle.Transaction;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<StatementOfWork>())
            {
                @this.DeniedPermissions = @this.TransitionalDeniedPermissions;

                var deletePermission = new Permissions(@this.Strategy.Transaction).Get(@this.Meta, @this.Meta.Delete);

                if (@this.IsDeletable())
                {
                    @this.RemoveDeniedPermission(deletePermission);
                }
                else
                {
                    @this.AddDeniedPermission(deletePermission);
                }
            }
        }
    }
}
