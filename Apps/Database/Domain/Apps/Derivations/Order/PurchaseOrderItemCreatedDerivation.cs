// <copyright file="PurchaseOrderItemCreatedDerivation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Derivations;
    using Meta;
    using Database.Derivations;
    using Resources;

    public class PurchaseOrderItemCreatedDerivation : DomainDerivation
    {
        public PurchaseOrderItemCreatedDerivation(M m) : base(m, new Guid("7559bffd-7685-4023-bef7-9f5ff96b6f41")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.PurchaseOrderItem.PurchaseOrderItemState),
                new RolePattern(m.PurchaseOrderItem.AssignedDeliveryDate),
                new RolePattern(m.PurchaseOrderItem.AssignedVatRegime),
                new RolePattern(m.PurchaseOrderItem.AssignedIrpfRegime),
                new AssociationPattern(m.PurchaseOrder.PurchaseOrderItems),
                new RolePattern(m.PurchaseOrder.DeliveryDate) { Steps =  new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems } },
                new RolePattern(m.PurchaseOrder.DerivedVatRegime) { Steps =  new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems } },
                new RolePattern(m.PurchaseOrder.DerivedIrpfRegime) { Steps =  new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems } },
                new RolePattern(m.PurchaseOrder.OrderDate) { Steps =  new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems } },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;
            var transaction = cycle.Transaction;

            foreach (var @this in matches.Cast<PurchaseOrderItem>().Where(v => v.PurchaseOrderItemState.IsCreated))
            {
                var order = @this.PurchaseOrderWherePurchaseOrderItem;

                @this.DerivedDeliveryDate = @this.AssignedDeliveryDate ?? order?.DeliveryDate;
                @this.DerivedVatRegime = @this.AssignedVatRegime ?? order?.DerivedVatRegime;
                @this.VatRate = @this.DerivedVatRegime?.VatRates.First(v => v.FromDate <= order.OrderDate && (!v.ExistThroughDate || v.ThroughDate >= order.OrderDate));
                @this.DerivedIrpfRegime = @this.AssignedIrpfRegime ?? order?.DerivedIrpfRegime;
                @this.IrpfRate = @this.DerivedIrpfRegime?.IrpfRates.First(v => v.FromDate <= order.OrderDate && (!v.ExistThroughDate || v.ThroughDate >= order.OrderDate));
            }
        }
    }
}
