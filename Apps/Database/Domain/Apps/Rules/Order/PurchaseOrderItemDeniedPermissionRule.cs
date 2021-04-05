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

    public class PurchaseOrderItemDeniedPermissionRule : Rule
    {
        public PurchaseOrderItemDeniedPermissionRule(MetaPopulation m) : base(m, new Guid("68b556f7-00ae-49a7-8d51-49c52ae18b4d")) =>
            this.Patterns = new Pattern[]
        {
            new RolePattern(m.PurchaseOrderItem, m.PurchaseOrderItem.TransitionalDeniedPermissions),
            new RolePattern(m.OrderItemBilling, m.OrderItemBilling.OrderItem) { Steps = new IPropertyType[] { m.OrderItemBilling.OrderItem}, OfType = m.PurchaseOrderItem },
            new RolePattern(m.OrderRequirementCommitment, m.OrderRequirementCommitment.OrderItem) { Steps = new IPropertyType[] { m.OrderRequirementCommitment.OrderItem}, OfType = m.PurchaseOrderItem},
            new AssociationPattern(m.WorkEffort.OrderItemFulfillment) { OfType = m.PurchaseOrderItem },
            new AssociationPattern(m.OrderShipment.OrderItem) { OfType = m.PurchaseOrderItem },
        };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var transaction = cycle.Transaction;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<PurchaseOrderItem>())
            {
                @this.DeniedPermissions = @this.TransitionalDeniedPermissions;

                var deletePermission = new Permissions(@this.Strategy.Transaction).Get(@this.Meta, @this.Meta.Delete);
                if (@this.IsDeletable)
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
