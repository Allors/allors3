// <copyright file="SalesInvoice.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.State;

    public partial class SalesInvoice
    {
        private bool IsDeletable =>
                    this.SalesInvoiceState.Equals(new SalesInvoiceStates(this.Strategy.Session).ReadyForPosting) &&
            this.SalesInvoiceItems.All(v => v.IsDeletable) &&
            !this.ExistSalesOrders &&
            !this.ExistPurchaseInvoice &&
            !this.ExistRepeatingSalesInvoiceWhereSource &&
            !this.IsRepeatingInvoice;

        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.SalesInvoice, this.M.SalesInvoice.SalesInvoiceState),
        };

        public int PaymentNetDays
        {
            get
            {
                if (this.ExistSalesTerms)
                {
                    foreach (AgreementTerm term in this.SalesTerms)
                    {
                        if (term.TermType.Equals(new InvoiceTermTypes(this.Strategy.Session).PaymentNetDays))
                        {
                            if (int.TryParse(term.TermValue, out var netDays))
                            {
                                return netDays;
                            }
                        }
                    }
                }

                var now = this.Session().Now();
                var customerRelationship = this.BillToCustomer?.CustomerRelationshipsWhereCustomer
                    .FirstOrDefault(v => Equals(v.InternalOrganisation, this.BilledFrom)
                      && v.FromDate <= now
                      && (!v.ExistThroughDate || v.ThroughDate >= now));

                if (customerRelationship?.PaymentNetDays().HasValue == true)
                {
                    return customerRelationship.PaymentNetDays().Value;
                }

                if (this.ExistStore && this.Store.ExistPaymentNetDays)
                {
                    return this.Store.PaymentNetDays;
                }

                return 0;
            }
        }

        public InvoiceItem[] InvoiceItems => this.SalesInvoiceItems;

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistSalesInvoiceState)
            {
                this.SalesInvoiceState = new SalesInvoiceStates(this.Strategy.Session).ReadyForPosting;
            }

            if (!this.ExistEntryDate)
            {
                this.EntryDate = this.Session().Now();
            }

            if (!this.ExistInvoiceDate)
            {
                this.InvoiceDate = this.Session().Now();
            }

            if (this.ExistBillToCustomer)
            {
                this.PreviousBillToCustomer = this.BillToCustomer;
            }

            if (!this.ExistSalesInvoiceType)
            {
                this.SalesInvoiceType = new SalesInvoiceTypes(this.Strategy.Session).SalesInvoice;
            }

            this.DefaultLocale = this.Session().GetSingleton().DefaultLocale;
            this.DefaultCurrency = this.Session().GetSingleton().DefaultLocale.Country.Currency;
        }

        public void AppsOnInit(ObjectOnInit method)
        {
            var internalOrganisations = new Organisations(this.Session()).InternalOrganisations();

            if (!this.ExistBilledFrom && internalOrganisations.Length == 1)
            {
                this.BilledFrom = internalOrganisations[0];
            }
        }

        public void AppsSend(SalesInvoiceSend method)
        {
            var singleton = this.Session().GetSingleton();

            if (object.Equals(this.SalesInvoiceType, new SalesInvoiceTypes(this.Strategy.Session).SalesInvoice))
            {
                this.InvoiceNumber = this.Store.NextInvoiceNumber(this.InvoiceDate.Year);
                this.SortableInvoiceNumber = NumberFormatter.SortableNumber(this.Store.SalesInvoiceNumberPrefix, this.InvoiceNumber, this.InvoiceDate.Year.ToString());
            }

            if (object.Equals(this.SalesInvoiceType, new SalesInvoiceTypes(this.Strategy.Session).CreditNote))
            {
                this.InvoiceNumber = this.Store.NextCreditNoteNumber(this.InvoiceDate.Year);
                this.SortableInvoiceNumber = NumberFormatter.SortableNumber(this.Store.CreditNoteNumberPrefix, this.InvoiceNumber, this.InvoiceDate.Year.ToString());
            }

            this.SalesInvoiceState = new SalesInvoiceStates(this.Strategy.Session).NotPaid;

            foreach (SalesInvoiceItem salesInvoiceItem in this.SalesInvoiceItems)
            {
                salesInvoiceItem.SalesInvoiceItemState = new SalesInvoiceItemStates(this.Strategy.Session).NotPaid;
                salesInvoiceItem.DerivationTrigger = Guid.NewGuid();

                if (this.SalesInvoiceType.Equals(new SalesInvoiceTypes(this.Session()).SalesInvoice)
                    && salesInvoiceItem.ExistSerialisedItem
                    && (this.BillToCustomer as InternalOrganisation)?.IsInternalOrganisation == false
                    && this.BilledFrom.SerialisedItemSoldOns.Contains(new SerialisedItemSoldOns(this.Session()).SalesInvoiceSend)
                    && salesInvoiceItem.NextSerialisedItemAvailability?.Equals(new SerialisedItemAvailabilities(this.Session()).Sold) == true)
                {
                    salesInvoiceItem.SerialisedItemVersionBeforeSale = salesInvoiceItem.SerialisedItem.CurrentVersion;

                    salesInvoiceItem.SerialisedItem.Seller = this.BilledFrom;
                    salesInvoiceItem.SerialisedItem.OwnedBy = this.BillToCustomer;
                    salesInvoiceItem.SerialisedItem.Ownership = new Ownerships(this.Session()).ThirdParty;
                    salesInvoiceItem.SerialisedItem.SerialisedItemAvailability = salesInvoiceItem.NextSerialisedItemAvailability;
                    salesInvoiceItem.SerialisedItem.AvailableForSale = false;
                }

                if (this.SalesInvoiceType.Equals(new SalesInvoiceTypes(this.Session()).CreditNote)
                    && salesInvoiceItem.ExistSerialisedItem
                    && salesInvoiceItem.ExistSerialisedItemVersionBeforeSale
                    && (this.BillToCustomer as InternalOrganisation)?.IsInternalOrganisation == false
                    && this.BilledFrom.SerialisedItemSoldOns.Contains(new SerialisedItemSoldOns(this.Session()).SalesInvoiceSend))
                {
                    salesInvoiceItem.SerialisedItem.Seller = salesInvoiceItem.SerialisedItemVersionBeforeSale.Seller;
                    salesInvoiceItem.SerialisedItem.OwnedBy = salesInvoiceItem.SerialisedItemVersionBeforeSale.OwnedBy;
                    salesInvoiceItem.SerialisedItem.Ownership = salesInvoiceItem.SerialisedItemVersionBeforeSale.Ownership;
                    salesInvoiceItem.SerialisedItem.SerialisedItemAvailability = salesInvoiceItem.SerialisedItemVersionBeforeSale.SerialisedItemAvailability;
                    salesInvoiceItem.SerialisedItem.AvailableForSale = salesInvoiceItem.SerialisedItemVersionBeforeSale.AvailableForSale;
                }
            }

            if (this.BillToCustomer is Organisation organisation && organisation.IsInternalOrganisation)
            {
                var purchaseInvoice = new PurchaseInvoiceBuilder(this.Strategy.Session)
                    .WithBilledFrom((Organisation)this.BilledFrom)
                    .WithBilledFromContactPerson(this.BilledFromContactPerson)
                    .WithBilledTo((InternalOrganisation)this.BillToCustomer)
                    .WithBilledToContactPerson(this.BillToContactPerson)
                    .WithBillToEndCustomer(this.BillToEndCustomer)
                    .WithBillToEndCustomerContactMechanism(this.BillToEndCustomerContactMechanism)
                    .WithBillToEndCustomerContactPerson(this.BillToEndCustomerContactPerson)
                    .WithBillToCustomerPaymentMethod(this.PaymentMethod)
                    .WithShipToCustomer(this.ShipToCustomer)
                    .WithShipToCustomerAddress(this.ShipToAddress)
                    .WithShipToCustomerContactPerson(this.ShipToContactPerson)
                    .WithShipToEndCustomer(this.ShipToEndCustomer)
                    .WithShipToEndCustomerAddress(this.ShipToEndCustomerAddress)
                    .WithShipToEndCustomerContactPerson(this.ShipToEndCustomerContactPerson)
                    .WithDescription(this.Description)
                    .WithInvoiceDate(this.Session().Now())
                    .WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.Strategy.Session).PurchaseInvoice)
                    .WithVatRegime(this.BilledFrom.VatRegime)
                    .WithIrpfRegime(this.BilledFrom.IrpfRegime)
                    .WithCustomerReference(this.CustomerReference)
                    .WithComment(this.Comment)
                    .WithInternalComment(this.InternalComment)
                    .Build();

                foreach (OrderAdjustment orderAdjustment in this.OrderAdjustments)
                {
                    OrderAdjustment newAdjustment = null;
                    if (orderAdjustment.GetType().Name.Equals(typeof(DiscountAdjustment).Name))
                    {
                        newAdjustment = new DiscountAdjustmentBuilder(this.Session()).Build();
                    }

                    if (orderAdjustment.GetType().Name.Equals(typeof(SurchargeAdjustment).Name))
                    {
                        newAdjustment = new SurchargeAdjustmentBuilder(this.Session()).Build();
                    }

                    if (orderAdjustment.GetType().Name.Equals(typeof(Fee).Name))
                    {
                        newAdjustment = new FeeBuilder(this.Session()).Build();
                    }

                    if (orderAdjustment.GetType().Name.Equals(typeof(ShippingAndHandlingCharge).Name))
                    {
                        newAdjustment = new ShippingAndHandlingChargeBuilder(this.Session()).Build();
                    }

                    if (orderAdjustment.GetType().Name.Equals(typeof(MiscellaneousCharge).Name))
                    {
                        newAdjustment = new MiscellaneousChargeBuilder(this.Session()).Build();
                    }

                    newAdjustment.Amount ??= orderAdjustment.Amount;
                    newAdjustment.Percentage ??= orderAdjustment.Percentage;
                    purchaseInvoice.AddOrderAdjustment(newAdjustment);
                }

                foreach (SalesInvoiceItem salesInvoiceItem in this.SalesInvoiceItems)
                {
                    var invoiceItem = new PurchaseInvoiceItemBuilder(this.Strategy.Session)
                        .WithInvoiceItemType(salesInvoiceItem.InvoiceItemType)
                        .WithAssignedUnitPrice(salesInvoiceItem.AssignedUnitPrice)
                        .WithAssignedVatRegime(salesInvoiceItem.AssignedVatRegime)
                        .WithAssignedIrpfRegime(salesInvoiceItem.AssignedIrpfRegime)
                        .WithPart(salesInvoiceItem.Product as UnifiedGood)
                        .WithSerialisedItem(salesInvoiceItem.SerialisedItem)
                        .WithQuantity(salesInvoiceItem.Quantity)
                        .WithDescription(salesInvoiceItem.Description)
                        .WithComment(salesInvoiceItem.Comment)
                        .WithInternalComment(salesInvoiceItem.InternalComment)
                        .Build();

                    purchaseInvoice.AddPurchaseInvoiceItem(invoiceItem);
                }
            }
        }

        public void AppsWriteOff(SalesInvoiceWriteOff method) => this.SalesInvoiceState = new SalesInvoiceStates(this.Strategy.Session).WrittenOff;

        public void AppsReopen(SalesInvoiceReopen method) => this.SalesInvoiceState = this.PreviousSalesInvoiceState;

        public void AppsCancelInvoice(SalesInvoiceCancelInvoice method) => this.SalesInvoiceState = new SalesInvoiceStates(this.Strategy.Session).Cancelled;

        public SalesInvoice AppsCopy(SalesInvoiceCopy method)
        {
            var salesInvoice = new SalesInvoiceBuilder(this.Strategy.Session)
                .WithPurchaseInvoice(this.PurchaseInvoice)
                .WithBilledFrom(this.BilledFrom)
                .WithBilledFromContactMechanism(this.BilledFromContactMechanism)
                .WithBilledFromContactPerson(this.BilledFromContactPerson)
                .WithBillToCustomer(this.BillToCustomer)
                .WithBillToContactMechanism(this.BillToContactMechanism)
                .WithBillToContactPerson(this.BillToContactPerson)
                .WithBillToEndCustomer(this.BillToEndCustomer)
                .WithBillToEndCustomerContactMechanism(this.BillToEndCustomerContactMechanism)
                .WithBillToEndCustomerContactPerson(this.BillToEndCustomerContactPerson)
                .WithShipToCustomer(this.ShipToCustomer)
                .WithShipToAddress(this.ShipToAddress)
                .WithShipToContactPerson(this.ShipToContactPerson)
                .WithShipToEndCustomer(this.ShipToEndCustomer)
                .WithShipToEndCustomerAddress(this.ShipToEndCustomerAddress)
                .WithShipToEndCustomerContactPerson(this.ShipToEndCustomerContactPerson)
                .WithDescription(this.Description)
                .WithStore(this.Store)
                .WithInvoiceDate(this.Session().Now())
                .WithSalesChannel(this.SalesChannel)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Strategy.Session).SalesInvoice)
                .WithVatRegime(this.VatRegime)
                .WithIrpfRegime(this.IrpfRegime)
                .WithCustomerReference(this.CustomerReference)
                .WithPaymentMethod(this.PaymentMethod)
                .WithComment(this.Comment)
                .WithInternalComment(this.InternalComment)
                .WithMessage(this.Message)
                .WithBillingAccount(this.BillingAccount)
                .Build();

            foreach (OrderAdjustment orderAdjustment in this.OrderAdjustments)
            {
                OrderAdjustment newAdjustment = null;
                if (orderAdjustment.GetType().Name.Equals(typeof(DiscountAdjustment).Name))
                {
                    newAdjustment = new DiscountAdjustmentBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(SurchargeAdjustment).Name))
                {
                    newAdjustment = new SurchargeAdjustmentBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(Fee).Name))
                {
                    newAdjustment = new FeeBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(ShippingAndHandlingCharge).Name))
                {
                    newAdjustment = new ShippingAndHandlingChargeBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(MiscellaneousCharge).Name))
                {
                    newAdjustment = new MiscellaneousChargeBuilder(this.Session()).Build();
                }

                newAdjustment.Amount ??= orderAdjustment.Amount;
                newAdjustment.Percentage ??= orderAdjustment.Percentage;
                salesInvoice.AddOrderAdjustment(newAdjustment);
            }

            foreach (SalesInvoiceItem salesInvoiceItem in this.SalesInvoiceItems)
            {
                var invoiceItem = new SalesInvoiceItemBuilder(this.Strategy.Session)
                    .WithInvoiceItemType(salesInvoiceItem.InvoiceItemType)
                    .WithAssignedUnitPrice(salesInvoiceItem.AssignedUnitPrice)
                    .WithAssignedVatRegime(salesInvoiceItem.AssignedVatRegime)
                    .WithAssignedIrpfRegime(salesInvoiceItem.AssignedIrpfRegime)
                    .WithProduct(salesInvoiceItem.Product)
                    .WithQuantity(salesInvoiceItem.Quantity)
                    .WithDescription(salesInvoiceItem.Description)
                    .WithSerialisedItem(salesInvoiceItem.SerialisedItem)
                    .WithNextSerialisedItemAvailability(salesInvoiceItem.NextSerialisedItemAvailability)
                    .WithComment(salesInvoiceItem.Comment)
                    .WithInternalComment(salesInvoiceItem.InternalComment)
                    .WithMessage(salesInvoiceItem.Message)
                    .WithFacility(salesInvoiceItem.Facility)
                    .Build();

                invoiceItem.ProductFeatures = salesInvoiceItem.ProductFeatures;
                salesInvoice.AddSalesInvoiceItem(invoiceItem);

                foreach (SalesTerm salesTerm in salesInvoiceItem.SalesTerms)
                {
                    if (salesTerm.GetType().Name == typeof(IncoTerm).Name)
                    {
                        salesInvoiceItem.AddSalesTerm(new IncoTermBuilder(this.Strategy.Session)
                            .WithTermType(salesTerm.TermType)
                            .WithTermValue(salesTerm.TermValue)
                            .WithDescription(salesTerm.Description)
                            .Build());
                    }

                    if (salesTerm.GetType().Name == typeof(InvoiceTerm).Name)
                    {
                        salesInvoiceItem.AddSalesTerm(new InvoiceTermBuilder(this.Strategy.Session)
                            .WithTermType(salesTerm.TermType)
                            .WithTermValue(salesTerm.TermValue)
                            .WithDescription(salesTerm.Description)
                            .Build());
                    }

                    if (salesTerm.GetType().Name == typeof(OrderTerm).Name)
                    {
                        salesInvoiceItem.AddSalesTerm(new OrderTermBuilder(this.Strategy.Session)
                            .WithTermType(salesTerm.TermType)
                            .WithTermValue(salesTerm.TermValue)
                            .WithDescription(salesTerm.Description)
                            .Build());
                    }
                }
            }

            foreach (SalesTerm salesTerm in this.SalesTerms)
            {
                if (salesTerm.GetType().Name == typeof(IncoTerm).Name)
                {
                    salesInvoice.AddSalesTerm(new IncoTermBuilder(this.Strategy.Session)
                                                .WithTermType(salesTerm.TermType)
                                                .WithTermValue(salesTerm.TermValue)
                                                .WithDescription(salesTerm.Description)
                                                .Build());
                }

                if (salesTerm.GetType().Name == typeof(InvoiceTerm).Name)
                {
                    salesInvoice.AddSalesTerm(new InvoiceTermBuilder(this.Strategy.Session)
                        .WithTermType(salesTerm.TermType)
                        .WithTermValue(salesTerm.TermValue)
                        .WithDescription(salesTerm.Description)
                        .Build());
                }

                if (salesTerm.GetType().Name == typeof(OrderTerm).Name)
                {
                    salesInvoice.AddSalesTerm(new OrderTermBuilder(this.Strategy.Session)
                        .WithTermType(salesTerm.TermType)
                        .WithTermValue(salesTerm.TermValue)
                        .WithDescription(salesTerm.Description)
                        .Build());
                }
            }

            return salesInvoice;
        }

        public SalesInvoice BaseCredit(SalesInvoiceCredit method)
        {
            var salesInvoice = new SalesInvoiceBuilder(this.Strategy.Session)
                .WithPurchaseInvoice(this.PurchaseInvoice)
                .WithBilledFrom(this.BilledFrom)
                .WithBilledFromContactMechanism(this.BilledFromContactMechanism)
                .WithBilledFromContactPerson(this.BilledFromContactPerson)
                .WithBillToCustomer(this.BillToCustomer)
                .WithBillToContactMechanism(this.BillToContactMechanism)
                .WithBillToContactPerson(this.BillToContactPerson)
                .WithBillToEndCustomer(this.BillToEndCustomer)
                .WithBillToEndCustomerContactMechanism(this.BillToEndCustomerContactMechanism)
                .WithBillToEndCustomerContactPerson(this.BillToEndCustomerContactPerson)
                .WithShipToCustomer(this.ShipToCustomer)
                .WithShipToAddress(this.ShipToAddress)
                .WithShipToContactPerson(this.ShipToContactPerson)
                .WithShipToEndCustomer(this.ShipToEndCustomer)
                .WithShipToEndCustomerAddress(this.ShipToEndCustomerAddress)
                .WithShipToEndCustomerContactPerson(this.ShipToEndCustomerContactPerson)
                .WithDescription(this.Description)
                .WithStore(this.Store)
                .WithInvoiceDate(this.Session().Now())
                .WithSalesChannel(this.SalesChannel)
                .WithSalesInvoiceType(new SalesInvoiceTypes(this.Strategy.Session).CreditNote)
                .WithVatRegime(this.VatRegime)
                .WithIrpfRegime(this.IrpfRegime)
                .WithCustomerReference(this.CustomerReference)
                .WithPaymentMethod(this.PaymentMethod)
                .WithBillingAccount(this.BillingAccount)
                .Build();

            foreach (OrderAdjustment orderAdjustment in this.OrderAdjustments)
            {
                OrderAdjustment newAdjustment = null;
                if (orderAdjustment.GetType().Name.Equals(typeof(DiscountAdjustment).Name))
                {
                    newAdjustment = new DiscountAdjustmentBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(SurchargeAdjustment).Name))
                {
                    newAdjustment = new SurchargeAdjustmentBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(Fee).Name))
                {
                    newAdjustment = new FeeBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(ShippingAndHandlingCharge).Name))
                {
                    newAdjustment = new ShippingAndHandlingChargeBuilder(this.Session()).Build();
                }

                if (orderAdjustment.GetType().Name.Equals(typeof(MiscellaneousCharge).Name))
                {
                    newAdjustment = new MiscellaneousChargeBuilder(this.Session()).Build();
                }

                newAdjustment.Amount ??= orderAdjustment.Amount * -1;
                salesInvoice.AddOrderAdjustment(newAdjustment);
            }

            foreach (SalesInvoiceItem salesInvoiceItem in this.SalesInvoiceItems)
            {
                var invoiceItem = new SalesInvoiceItemBuilder(this.Strategy.Session)
                    .WithInvoiceItemType(salesInvoiceItem.InvoiceItemType)
                    .WithAssignedUnitPrice(salesInvoiceItem.AssignedUnitPrice)
                    .WithProduct(salesInvoiceItem.Product)
                    .WithQuantity(salesInvoiceItem.Quantity)
                    .WithAssignedVatRegime(salesInvoiceItem.AssignedVatRegime)
                    .WithAssignedIrpfRegime(salesInvoiceItem.AssignedIrpfRegime)
                    .WithDescription(salesInvoiceItem.Description)
                    .WithSerialisedItem(salesInvoiceItem.SerialisedItem)
                    .WithComment(salesInvoiceItem.Comment)
                    .WithInternalComment(salesInvoiceItem.InternalComment)
                    .WithFacility(salesInvoiceItem.Facility)
                    .WithSerialisedItemVersionBeforeSale(salesInvoiceItem.SerialisedItemVersionBeforeSale)
                    .Build();

                invoiceItem.ProductFeatures = salesInvoiceItem.ProductFeatures;
                salesInvoice.AddSalesInvoiceItem(invoiceItem);
            }

            return salesInvoice;
        }

        public void AppsDelete(DeletableDelete method)
        {
            if (this.IsDeletable)
            {
                foreach (OrderAdjustment orderAdjustment in this.OrderAdjustments)
                {
                    orderAdjustment.Delete();
                }

                foreach (SalesInvoiceItem salesInvoiceItem in this.SalesInvoiceItems)
                {
                    salesInvoiceItem.Delete();
                }

                foreach (SalesTerm salesTerm in this.SalesTerms)
                {
                    salesTerm.Delete();
                }
            }
        }

        public void AppsPrint(PrintablePrint method)
        {
            if (!method.IsPrinted)
            {
                var singleton = this.Strategy.Session.GetSingleton();
                var logo = this.BilledFrom?.ExistLogoImage == true ?
                               this.BilledFrom.LogoImage.MediaContent.Data :
                               singleton.LogoImage.MediaContent.Data;

                var images = new Dictionary<string, byte[]>
                                 {
                                     { "Logo", logo },
                                 };

                if (this.ExistInvoiceNumber)
                {
                    var session = this.Strategy.Session;
                    var barcodeService = session.Database.State().BarcodeGenerator;
                    var barcode = barcodeService.Generate(this.InvoiceNumber, BarcodeType.CODE_128, 320, 80, pure: true);
                    images.Add("Barcode", barcode);
                }

                var printModel = new Print.SalesInvoiceModel.Model(this);
                this.RenderPrintDocument(this.BilledFrom?.SalesInvoiceTemplate, printModel, images);

                this.PrintDocument.Media.InFileName = $"{this.InvoiceNumber}.odt";
            }
        }
    }
}
