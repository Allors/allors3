// <copyright file="SalesInvoiceItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;

    public partial class SalesInvoiceItem
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.SalesInvoiceItem, this.M.SalesInvoiceItem.SalesInvoiceItemState),
        };

        public bool IsValid => !(this.SalesInvoiceItemState.IsCancelledByInvoice || this.SalesInvoiceItemState.IsWrittenOff);

        public decimal PriceAdjustment => this.TotalSurcharge - this.TotalDiscount;

        public decimal PriceAdjustmentAsPercentage => Math.Round((this.TotalSurcharge - this.TotalDiscount) / this.TotalBasePrice * 100, 2);

        public Part DerivedPart
        {
            get
            {
                if (this.ExistPart)
                {
                    return this.Part;
                }
                else
                {
                    if (this.ExistProduct)
                    {
                        var nonUnifiedGood = this.Product as NonUnifiedGood;
                        var unifiedGood = this.Product as UnifiedGood;
                        return unifiedGood ?? nonUnifiedGood?.Part;
                    }
                }

                return null;
            }
        }

        internal bool IsDeletable =>
            this.SalesInvoiceItemState.Equals(new SalesInvoiceItemStates(this.Strategy.Session).ReadyForPosting);

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
            if (!this.ExistSalesInvoiceItemState)
            {
                this.SalesInvoiceItemState = new SalesInvoiceItemStates(this.Strategy.Session).ReadyForPosting;
            }

            if (this.ExistProduct && !this.ExistInvoiceItemType)
            {
                this.InvoiceItemType = new InvoiceItemTypes(this.Strategy.Session).ProductItem;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            //var (iteration, changeSet, derivedObjects) = method;

            //if (iteration.IsMarked(this) || changeSet.IsCreated(this) || changeSet.HasChangedRoles(this))
            //{

            //    if (this.ExistSalesInvoiceWhereSalesInvoiceItem)
            //    {
            //        iteration.AddDependency(this.SalesInvoiceWhereSalesInvoiceItem, this);
            //        iteration.Mark(this.SalesInvoiceWhereSalesInvoiceItem);
            //    }

            //    if (this.ExistPaymentApplicationsWhereInvoiceItem)
            //    {
            //        foreach (PaymentApplication paymentApplication in this.PaymentApplicationsWhereInvoiceItem)
            //        {
            //            iteration.AddDependency(this, paymentApplication);
            //            iteration.Mark(paymentApplication);
            //        }
            //    }

            //    foreach (OrderItemBilling orderItemBilling in this.OrderItemBillingsWhereInvoiceItem)
            //    {
            //        iteration.AddDependency(orderItemBilling.OrderItem, this);
            //        iteration.Mark(orderItemBilling.OrderItem);
            //    }

            //    foreach (ShipmentItemBilling shipmentItemBilling in this.ShipmentItemBillingsWhereInvoiceItem)
            //    {
            //        foreach (OrderShipment orderShipment in shipmentItemBilling.ShipmentItem.OrderShipmentsWhereShipmentItem)
            //        {
            //            iteration.AddDependency(orderShipment.OrderItem, this);
            //            iteration.Mark(orderShipment.OrderItem);
            //        }
            //    }
            //}
        }

        public void AppsOnInit(ObjectOnInit method)
        {
            if (this.ExistProduct && !this.ExistInvoiceItemType)
            {
                this.InvoiceItemType = new InvoiceItemTypes(this.Strategy.Session).ProductItem;
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            //var derivation = method.Derivation;
            //var salesInvoice = this.SalesInvoiceWhereSalesInvoiceItem;
            //var salesInvoiceItemStates = new SalesInvoiceItemStates(derivation.Session);

            //derivation.Validation.AssertExistsAtMostOne(this, M.SalesInvoiceItem.Product, M.SalesInvoiceItem.ProductFeatures, M.SalesInvoiceItem.Part);
            //derivation.Validation.AssertExistsAtMostOne(this, M.SalesInvoiceItem.SerialisedItem, M.SalesInvoiceItem.ProductFeatures, M.SalesInvoiceItem.Part);

            //if (this.ExistSerialisedItem && !this.ExistNextSerialisedItemAvailability && salesInvoice.SalesInvoiceType.Equals(new SalesInvoiceTypes(this.Session()).SalesInvoice))
            //{
            //    derivation.Validation.AssertExists(this, this.Meta.NextSerialisedItemAvailability);
            //}

            //if (this.Part != null && this.Part.InventoryItemKind.IsSerialised && this.Quantity != 1)
            //{
            //    derivation.Validation.AddError(this, M.SalesInvoiceItem.Quantity, ErrorMessages.InvalidQuantity);
            //}

            //if (this.Part != null && this.Part.InventoryItemKind.IsNonSerialised && this.Quantity == 0)
            //{
            //    derivation.Validation.AddError(this, M.SalesInvoiceItem.Quantity, ErrorMessages.InvalidQuantity);
            //}

            //if (this.ExistInvoiceItemType && this.InvoiceItemType.MaxQuantity.HasValue && this.Quantity > this.InvoiceItemType.MaxQuantity.Value)
            //{
            //    derivation.Validation.AddError(this, M.SalesInvoiceItem.Quantity, ErrorMessages.InvalidQuantity);
            //}

            //this.VatRegime = this.ExistAssignedVatRegime ? this.AssignedVatRegime : this.SalesInvoiceWhereSalesInvoiceItem?.VatRegime;
            //this.VatRate = this.VatRegime?.VatRate;

            //this.IrpfRegime = this.ExistAssignedIrpfRegime ? this.AssignedIrpfRegime : this.SalesInvoiceWhereSalesInvoiceItem?.IrpfRegime;
            //this.IrpfRate = this.IrpfRegime?.IrpfRate;

            //if (this.ExistInvoiceItemType && this.IsSubTotalItem().Result == true && this.Quantity <= 0)
            //{
            //    derivation.Validation.AssertExists(this, this.Meta.Quantity);
            //}

            //this.AmountPaid = 0;
            //foreach (PaymentApplication paymentApplication in this.PaymentApplicationsWhereInvoiceItem)
            //{
            //    this.AmountPaid += paymentApplication.AmountApplied;
            //}

            //if (salesInvoice != null && salesInvoice.SalesInvoiceState.IsReadyForPosting && this.SalesInvoiceItemState.IsCancelledByInvoice)
            //{
            //    this.SalesInvoiceItemState = salesInvoiceItemStates.ReadyForPosting;
            //}

            //// SalesInvoiceItem States
            //if (salesInvoice != null && this.IsValid)
            //{
            //    if (salesInvoice.SalesInvoiceState.IsWrittenOff)
            //    {
            //        this.SalesInvoiceItemState = salesInvoiceItemStates.WrittenOff;
            //    }

            //    if (salesInvoice.SalesInvoiceState.IsCancelled)
            //    {
            //        this.SalesInvoiceItemState = salesInvoiceItemStates.CancelledByInvoice;
            //    }
            //}

            //// TODO: Move to Custom
            //if (derivation.ChangeSet.IsCreated(this) && !this.ExistDescription)
            //{
            //    if (this.ExistSerialisedItem)
            //    {
            //        var builder = new StringBuilder();
            //        var part = this.SerialisedItem.PartWhereSerialisedItem;

            //        if (part != null && part.ExistManufacturedBy)
            //        {
            //            builder.Append($", Manufacturer: {part.ManufacturedBy.PartyName}");
            //        }

            //        if (part != null && part.ExistBrand)
            //        {
            //            builder.Append($", Brand: {part.Brand.Name}");
            //        }

            //        if (part != null && part.ExistModel)
            //        {
            //            builder.Append($", Model: {part.Model.Name}");
            //        }

            //        builder.Append($", SN: {this.SerialisedItem.SerialNumber}");

            //        if (this.SerialisedItem.ExistManufacturingYear)
            //        {
            //            builder.Append($", YOM: {this.SerialisedItem.ManufacturingYear}");
            //        }

            //        foreach (SerialisedItemCharacteristic characteristic in this.SerialisedItem.SerialisedItemCharacteristics)
            //        {
            //            if (characteristic.ExistValue)
            //            {
            //                var characteristicType = characteristic.SerialisedItemCharacteristicType;
            //                if (characteristicType.ExistUnitOfMeasure)
            //                {
            //                    var uom = characteristicType.UnitOfMeasure.ExistAbbreviation
            //                                    ? characteristicType.UnitOfMeasure.Abbreviation
            //                                    : characteristicType.UnitOfMeasure.Name;
            //                    builder.Append(
            //                        $", {characteristicType.Name}: {characteristic.Value} {uom}");
            //                }
            //                else
            //                {
            //                    builder.Append($", {characteristicType.Name}: {characteristic.Value}");
            //                }
            //            }
            //        }

            //        var details = builder.ToString();

            //        if (details.StartsWith(","))
            //        {
            //            details = details.Substring(2);
            //        }

            //        this.Description = details;

            //    }
            //    else if (this.ExistProduct && this.Product is UnifiedGood unifiedGood)
            //    {
            //        var builder = new StringBuilder();

            //        if (unifiedGood != null && unifiedGood.ExistManufacturedBy)
            //        {
            //            builder.Append($", Manufacturer: {unifiedGood.ManufacturedBy.PartyName}");
            //        }

            //        if (unifiedGood != null && unifiedGood.ExistBrand)
            //        {
            //            builder.Append($", Brand: {unifiedGood.Brand.Name}");
            //        }

            //        if (unifiedGood != null && unifiedGood.ExistModel)
            //        {
            //            builder.Append($", Model: {unifiedGood.Model.Name}");
            //        }

            //        foreach (SerialisedItemCharacteristic characteristic in unifiedGood.SerialisedItemCharacteristics)
            //        {
            //            if (characteristic.ExistValue)
            //            {
            //                var characteristicType = characteristic.SerialisedItemCharacteristicType;
            //                if (characteristicType.ExistUnitOfMeasure)
            //                {
            //                    var uom = characteristicType.UnitOfMeasure.ExistAbbreviation
            //                                    ? characteristicType.UnitOfMeasure.Abbreviation
            //                                    : characteristicType.UnitOfMeasure.Name;
            //                    builder.Append($", {characteristicType.Name}: {characteristic.Value} {uom}");
            //                }
            //                else
            //                {
            //                    builder.Append($", {characteristicType.Name}: {characteristic.Value}");
            //                }
            //            }
            //        }

            //        var details = builder.ToString();

            //        if (details.StartsWith(","))
            //        {
            //            details = details.Substring(2);
            //        }

            //        this.Description = details;
            //    }
            //}
        }

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            //var deletePermission = new Permissions(this.Strategy.Session).Get(this.Meta.ObjectType, this.Meta.Delete);
            //if (this.IsDeletable)
            //{
            //    this.RemoveDeniedPermission(deletePermission);
            //}
            //else
            //{
            //    this.AddDeniedPermission(deletePermission);
            //}
        }

        public void AppsWriteOff() => this.SalesInvoiceItemState = new SalesInvoiceItemStates(this.Strategy.Session).WrittenOff;

        public void CancelFromInvoice() => this.SalesInvoiceItemState = new SalesInvoiceItemStates(this.Strategy.Session).CancelledByInvoice;

        public void AppsDelete(DeletableDelete method)
        {
            foreach (SalesTerm salesTerm in this.SalesTerms)
            {
                salesTerm.Delete();
            }

            foreach (InvoiceVatRateItem invoiceVatRateItem in this.InvoiceVatRateItems)
            {
                invoiceVatRateItem.Delete();
            }

            foreach (WorkEffortBilling billing in this.WorkEffortBillingsWhereInvoiceItem)
            {
                billing.WorkEffort.DerivationTrigger = Guid.NewGuid();
                billing.Delete();
            }

            foreach (OrderItemBilling billing in this.OrderItemBillingsWhereInvoiceItem)
            {
                billing.OrderItem.DerivationTrigger = Guid.NewGuid();
                billing.Delete();
            }

            foreach (ShipmentItemBilling billing in this.ShipmentItemBillingsWhereInvoiceItem)
            {
                billing.ShipmentItem.DerivationTrigger = Guid.NewGuid();
                billing.Delete();
            }

            foreach (TimeEntryBilling billing in this.TimeEntryBillingsWhereInvoiceItem)
            {
                billing.TimeEntry.WorkEffort.DerivationTrigger = Guid.NewGuid();
                billing.Delete();
            }

            foreach (ServiceEntryBilling billing in this.ServiceEntryBillingsWhereInvoiceItem)
            {
                billing.ServiceEntry.DerivationTrigger = Guid.NewGuid();
                billing.Delete();
            }
        }

        public void AppsIsSubTotalItem(SalesInvoiceItemIsSubTotalItem method)
        {
            if (!method.Result.HasValue)
            {
                method.Result = this.InvoiceItemType.Equals(new InvoiceItemTypes(this.Strategy.Session).ProductItem)
                    || this.InvoiceItemType.Equals(new InvoiceItemTypes(this.Strategy.Session).PartItem);
            }
        }

        public void Sync(Invoice invoice) => this.SyncedInvoice = invoice;
    }
}
