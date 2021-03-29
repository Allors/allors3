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

    public class SalesOrderItemProvisionalRule : Rule
    {
        public SalesOrderItemProvisionalRule(M m) : base(m, new Guid("2d5fad32-da2f-436a-a4fa-04b3a6f1b894")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.SalesOrderItemState),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedShipFromAddress),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedShipToAddress),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedShipToParty),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedDeliveryDate),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedVatRegime),
                new RolePattern(m.SalesOrderItem, m.SalesOrderItem.AssignedIrpfRegime),
                new AssociationPattern(m.SalesOrder.SalesOrderItems),
                new RolePattern(m.SalesOrder, m.SalesOrder.DerivedShipFromAddress) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.DerivedShipToAddress) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.ShipToCustomer) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.DeliveryDate) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.DerivedVatRegime) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.DerivedIrpfRegime) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.SalesOrder, m.SalesOrder.OrderDate) { Steps =  new IPropertyType[] {m.SalesOrder.SalesOrderItems} },
                new RolePattern(m.Organisation, m.Organisation.ShippingAddress) { Steps = new IPropertyType[] { m.Organisation.SalesOrderItemsWhereAssignedShipToParty  }},
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;
            var transaction = cycle.Transaction;

            foreach (var @this in matches.Cast<SalesOrderItem>().Where(v => v.SalesOrderItemState.IsProvisional))
            {
                var salesOrder = @this.SalesOrderWhereSalesOrderItem;

                @this.DerivedShipFromAddress = @this.AssignedShipFromAddress ?? salesOrder?.DerivedShipFromAddress;
                @this.DerivedShipToAddress = @this.AssignedShipToAddress ?? @this.AssignedShipToParty?.ShippingAddress ?? salesOrder?.DerivedShipToAddress;
                @this.DerivedShipToParty = @this.AssignedShipToParty ?? salesOrder?.ShipToCustomer;
                @this.DerivedDeliveryDate = @this.AssignedDeliveryDate ?? salesOrder?.DeliveryDate;
                @this.DerivedVatRegime = @this.AssignedVatRegime ?? salesOrder?.DerivedVatRegime;
                @this.VatRate = @this.DerivedVatRegime?.VatRates.First(v => v.FromDate <= salesOrder.OrderDate && (!v.ExistThroughDate || v.ThroughDate >= salesOrder.OrderDate));
                @this.DerivedIrpfRegime = @this.AssignedIrpfRegime ?? salesOrder?.DerivedIrpfRegime;
                @this.IrpfRate = @this.DerivedIrpfRegime?.IrpfRates.First(v => v.FromDate <= salesOrder.OrderDate && (!v.ExistThroughDate || v.ThroughDate >= salesOrder.OrderDate));
            }
        }
    }
}