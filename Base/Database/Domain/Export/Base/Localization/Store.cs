// <copyright file="Store.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System.Linq;
    using System;
    using Allors.Meta;

    public partial class Store
    {
        public string DeriveNextInvoiceNumber(int year)
        {
            int salesInvoiceNumber;
            if (this.InternalOrganisation.InvoiceSequence.Equals(new InvoiceSequences(this.Strategy.Session).EnforcedSequence))
            {
                salesInvoiceNumber = this.SalesInvoiceCounter.NextValue();
            }
            else
            {
                var fiscalYearInvoiceNumbers = new FiscalYearInvoiceNumbers(this.Strategy.Session).Extent();
                fiscalYearInvoiceNumbers.Filter.AddEquals(M.FiscalYearInvoiceNumber.FiscalYear, year);
                var fiscalYearInvoiceNumber = fiscalYearInvoiceNumbers.First;

                if (fiscalYearInvoiceNumber == null)
                {
                    fiscalYearInvoiceNumber = new FiscalYearInvoiceNumberBuilder(this.Strategy.Session).WithFiscalYear(year).Build();
                    fiscalYearInvoiceNumber.NextSalesInvoiceNumber = 1;
                    this.AddFiscalYearInvoiceNumber(fiscalYearInvoiceNumber);
                }

                salesInvoiceNumber = fiscalYearInvoiceNumber.DeriveNextSalesInvoiceNumber();
            }

            return string.Concat(this.SalesInvoiceNumberPrefix, salesInvoiceNumber).Replace("{year}", year.ToString());
        }

        public string DeriveNextTemporaryInvoiceNumber() => this.SalesInvoiceTemporaryCounter.NextValue().ToString();

        // TODO: Cascading delete
        // public override void RemovePaymentMethod(PaymentMethod value)
        // {
        // if (value.Equals(this.DefaultPaymentMethod))
        // {
        // this.RemoveDefaultPaymentMethod();
        // }

        // base.RemovePaymentMethod(value);
        // }
        public string DeriveNextShipmentNumber()
        {
            var shipmentNumber = this.OutgoingShipmentCounter.NextValue();
            return string.Concat(this.OutgoingShipmentNumberPrefix, shipmentNumber);
        }

        public string DeriveNextSalesOrderNumber(int year)
        {
            var salesOrderNumber = this.SalesOrderCounter.NextValue();
            return string.Concat(this.SalesOrderNumberPrefix, salesOrderNumber).Replace("{year}", year.ToString());
        }

        public string DeriveNextCreditNoteNumber(int year)
        {
            int creditNoteNumber;
            if (this.InternalOrganisation.InvoiceSequence.Equals(new InvoiceSequences(this.Strategy.Session).EnforcedSequence))
            {
                creditNoteNumber = this.CreditNoteCounter.NextValue();
            }
            else
            {
                var fiscalYearInvoiceNumbers = new FiscalYearInvoiceNumbers(this.Strategy.Session).Extent();
                fiscalYearInvoiceNumbers.Filter.AddEquals(M.FiscalYearInvoiceNumber.FiscalYear, year);
                var fiscalYearInvoiceNumber = fiscalYearInvoiceNumbers.First;

                if (fiscalYearInvoiceNumber == null)
                {
                    fiscalYearInvoiceNumber = new FiscalYearInvoiceNumberBuilder(this.Strategy.Session).WithFiscalYear(year).Build();
                    this.AddFiscalYearInvoiceNumber(fiscalYearInvoiceNumber);
                }

                creditNoteNumber = fiscalYearInvoiceNumber.DeriveNextCreditNoteNumber();
            }

            return string.Concat(this.CreditNoteNumberPrefix, creditNoteNumber).Replace("{year}", year.ToString());
        }

        public void BaseOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistSalesOrderCounter)
            {
                this.SalesOrderCounter = new CounterBuilder(this.Strategy.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            }

            if (!this.ExistOutgoingShipmentCounter)
            {
                this.OutgoingShipmentCounter = new CounterBuilder(this.Strategy.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            }

            //if (!this.ExistWorkEffortCounter)
            //{
            //    this.WorkEffortCounter = new CounterBuilder(this.Strategy.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            //}

            if (!this.ExistBillingProcess)
            {
                this.BillingProcess = new BillingProcesses(this.Strategy.Session).BillingForShipmentItems;
            }

            if (!this.ExistSalesInvoiceTemporaryCounter)
            {
                this.SalesInvoiceTemporaryCounter = new CounterBuilder(this.Strategy.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            }

            if (!this.ExistCreditNoteCounter)
            {
                this.CreditNoteCounter = new CounterBuilder(this.Strategy.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            }
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            var internalOrganisations = new Organisations(this.Strategy.Session).Extent().Where(v => Equals(v.IsInternalOrganisation, true)).ToArray();

            if (!this.ExistInternalOrganisation && internalOrganisations.Count() == 1)
            {
                this.InternalOrganisation = internalOrganisations.First();
            }

            if (this.ExistDefaultCollectionMethod && !this.CollectionMethods.Contains(this.DefaultCollectionMethod))
            {
                this.AddCollectionMethod(this.DefaultCollectionMethod);
            }

            if (!this.ExistDefaultCollectionMethod && this.CollectionMethods.Count == 1)
            {
                this.DefaultCollectionMethod = this.CollectionMethods.First;
            }

            if (!this.ExistDefaultCollectionMethod && this.InternalOrganisation.ExistDefaultCollectionMethod)
            {
                this.DefaultCollectionMethod = this.InternalOrganisation.DefaultCollectionMethod;

                if (!this.ExistCollectionMethods || !this.CollectionMethods.Contains(this.DefaultCollectionMethod))
                {
                    this.AddCollectionMethod(this.DefaultCollectionMethod);
                }
            }

            if (!this.ExistDefaultFacility)
            {
                this.DefaultFacility = this.Strategy.Session.GetSingleton().Settings.DefaultFacility;
            }

            derivation.Validation.AssertExistsAtMostOne(this, M.Store.FiscalYearInvoiceNumbers, M.Store.SalesInvoiceCounter);
        }
    }
}
