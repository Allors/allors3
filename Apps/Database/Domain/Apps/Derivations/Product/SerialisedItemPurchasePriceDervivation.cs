// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Database.Meta;
    using Database.Derivations;

    public class SerialisedItemPurchasePriceDervivation : DomainDerivation
    {
        public SerialisedItemPurchasePriceDervivation(M m) : base(m, new Guid("d9748a88-862d-4793-8fa2-0e052c6c13c9")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(m.PurchaseOrderItem.SerialisedItem) { Steps = new IPropertyType[] {m.PurchaseOrderItem.SerialisedItem} },
                new ChangedPattern(m.PurchaseInvoice.PurchaseInvoiceItems) { Steps = new IPropertyType[] {m.PurchaseInvoiceItem.SerialisedItem } },
                new ChangedPattern(m.PurchaseInvoice.ValidInvoiceItems) { Steps = new IPropertyType[] { m.PurchaseInvoice.ValidInvoiceItems, m.PurchaseInvoice.SerialisedItemsWherePurchaseInvoice } },
                new ChangedPattern(m.PurchaseOrder.ValidOrderItems) { Steps = new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems, m.PurchaseOrderItem.SerialisedItem} },
                new ChangedPattern(m.PurchaseOrder.PurchaseOrderState) { Steps = new IPropertyType[] {m.PurchaseOrder.PurchaseOrderItems, m.PurchaseOrderItem.SerialisedItem} },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<SerialisedItem>())
            {
                if (@this.ExistPurchaseInvoice)
                {
                    @this.PurchasePrice = @this.PurchaseInvoiceItemsWhereSerialisedItem
                        .LastOrDefault(v => v.ExistInvoiceWhereValidInvoiceItem
                                            && (v.InvoiceItemType.Equals(new InvoiceItemTypes(@this.Session()).PartItem)
                                                || v.InvoiceItemType.Equals(new InvoiceItemTypes(@this.Session()).ProductItem)))?
                        .UnitPrice ?? 0M;

                    @this.RemoveAssignedPurchasePrice();
                }
            }
        }
    }
}
