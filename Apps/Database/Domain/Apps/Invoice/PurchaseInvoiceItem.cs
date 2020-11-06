// <copyright file="PurchaseInvoiceItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;

    public partial class PurchaseInvoiceItem
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.PurchaseInvoiceItem, this.M.PurchaseInvoiceItem.PurchaseInvoiceItemState),
        };

        public bool IsValid => !(this.PurchaseInvoiceItemState.IsCancelledByInvoice || this.PurchaseInvoiceItemState.IsRejected);

        public decimal PriceAdjustment => this.TotalSurcharge - this.TotalDiscount;

        public void AppsDelegateAccess(DelegatedAccessControlledObjectDelegateAccess method)
        {
            if (method.SecurityTokens == null)
            {
                method.SecurityTokens = this.SyncedInvoice?.SecurityTokens.ToArray();
            }

            if (method.DeniedPermissions == null)
            {
                method.DeniedPermissions = this.SyncedInvoice?.DeniedPermissions.ToArray();
            }
        }

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistPurchaseInvoiceItemState)
            {
                this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Created;
            }

            if (this.ExistPart && !this.ExistInvoiceItemType)
            {
                this.InvoiceItemType = new InvoiceItemTypes(this.Strategy.Session).PartItem;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            var (iteration, changeSet, derivedObjects) = method;
            var invoice = this.PurchaseInvoiceWherePurchaseInvoiceItem;

            if (invoice != null && iteration.ChangeSet.Associations.Contains(this.Id))
            {
                iteration.AddDependency(invoice, this);
                iteration.Mark(invoice);
            }

            if (iteration.IsMarked(this) || changeSet.IsCreated(this) || changeSet.HasChangedRoles(this))
            {
                iteration.AddDependency(this.SerialisedItem, this);
                iteration.Mark(this.SerialisedItem);
            }

            foreach (OrderItemBilling orderItemBilling in this.OrderItemBillingsWhereInvoiceItem)
            {
                iteration.AddDependency(orderItemBilling.OrderItem, this);
                iteration.Mark(orderItemBilling.OrderItem);
            }
        }

        public void CancelFromInvoice() => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).CancelledByinvoice;

        public void AppsDelete(DeletableDelete method)
        {
            if (this.PurchaseInvoiceWherePurchaseInvoiceItem.PurchaseInvoiceState.IsCreated)
            {
                this.PurchaseInvoiceWherePurchaseInvoiceItem.RemovePurchaseInvoiceItem(this);
                foreach (OrderItemBilling orderItemBilling in this.OrderItemBillingsWhereInvoiceItem)
                {
                    orderItemBilling.OrderItem.DerivationTrigger = Guid.NewGuid();
                    orderItemBilling.Delete();
                }
            }
        }

        public void AppsReject(PurchaseInvoiceItemReject method) => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Rejected;

        public void AppsRevise(PurchaseInvoiceItemRevise method) => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Revising;

        public void AppsFinishRevising(PurchaseInvoiceItemFinishRevising method) => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Created;
    }
}
