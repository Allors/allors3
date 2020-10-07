// <copyright file="WorkEffortExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Linq;
    using Allors.Meta;

    public static partial class WorkEffortExtensions
    {
        public static DateTime? FromDate(this WorkEffort @this) => @this.ActualStart ?? @this.ScheduledStart;

        public static DateTime? ThroughDate(this WorkEffort @this) => @this.ActualCompletion ?? @this.ScheduledCompletion;

        public static TimeEntry[] BillableTimeEntries(this WorkEffort @this) => @this.ServiceEntriesWhereWorkEffort.OfType<TimeEntry>()
            .Where(v => v.IsBillable
                        && (!v.BillableAmountOfTime.HasValue && v.AmountOfTime.HasValue) || v.BillableAmountOfTime.HasValue)
            .Select(v => v)
            .ToArray();

        public static void BaseOnBuild(this WorkEffort @this, ObjectOnBuild method)
        {
            if (!@this.ExistWorkEffortState)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Created;
            }

            if (!@this.ExistOwner && @this.Strategy.Session.State().User is Person owner)
            {
                @this.Owner = owner;
            }
        }

        public static void BaseOnDerive(this WorkEffort @this, ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            if (!@this.ExistDerivationTrigger)
            {
                @this.DerivationTrigger = Guid.NewGuid();
            }

            var internalOrganisations = new Organisations(@this.Strategy.Session).Extent().Where(v => Equals(v.IsInternalOrganisation, true)).ToArray();

            if (!@this.ExistTakenBy && internalOrganisations.Count() == 1)
            {
                @this.TakenBy = internalOrganisations.First();
            }

            if (!@this.ExistWorkEffortNumber && @this.ExistTakenBy)
            {
                @this.WorkEffortNumber = @this.TakenBy.NextWorkEffortNumber();
                @this.SortableWorkEffortNumber = @this.Session().GetSingleton().SortableNumber(@this.TakenBy.WorkEffortPrefix, @this.WorkEffortNumber, @this.CreationDate.Value.Year.ToString());
            }

            if (!@this.ExistExecutedBy && @this.ExistTakenBy)
            {
                @this.ExecutedBy = @this.TakenBy;
            }

            @this.VerifyWorkEffortPartyAssignments(derivation);
            @this.DeriveActualHoursAndDates();

            if (!@this.ExistWorkEffortBillingsWhereWorkEffort
                && !@this.ExistServiceEntriesWhereWorkEffort
                && @this.WorkEffortState.IsFinished)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Completed;
            }

            if (@this.ExistActualStart && @this.WorkEffortState.IsCreated)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).InProgress;
            }

            @this.DeriveCanInvoice();

            foreach (WorkEffortInventoryAssignment inventoryAssignment in @this.WorkEffortInventoryAssignmentsWhereAssignment)
            {
                foreach (InventoryTransactionReason createReason in @this.WorkEffortState.InventoryTransactionReasonsToCreate)
                {
                    inventoryAssignment.SyncInventoryTransactions(derivation, inventoryAssignment.InventoryItem, inventoryAssignment.Quantity, createReason, false);
                }

                foreach (InventoryTransactionReason cancelReason in @this.WorkEffortState.InventoryTransactionReasonsToCancel)
                {
                    inventoryAssignment.SyncInventoryTransactions(derivation, inventoryAssignment.InventoryItem, inventoryAssignment.Quantity, cancelReason, true);
                }
            }
        }

        public static void BaseOnPostDerive(this WorkEffort @this, ObjectOnPostDerive method)
        {
            var m = @this.Strategy.Session.Database.State().M;

            if (!@this.CanInvoice)
            {
                @this.AddDeniedPermission(new Permissions(@this.Strategy.Session).Get((Class)@this.Strategy.Class, m.WorkEffort.Invoice));
            }
            else
            {
                @this.RemoveDeniedPermission(new Permissions(@this.Strategy.Session).Get((Class)@this.Strategy.Class, m.WorkEffort.Invoice));
            }

            var completePermission = new Permissions(@this.Strategy.Session).Get((Class)@this.Strategy.Class, m.WorkEffort.Complete);

            if (@this.ServiceEntriesWhereWorkEffort.Any(v => !v.ExistThroughDate))
            {
                @this.AddDeniedPermission(new Permissions(@this.Strategy.Session).Get((Class)@this.Strategy.Class, m.WorkEffort.Complete));
            }
            else
            {
                if (@this.WorkEffortState.IsInProgress)
                {
                    @this.RemoveDeniedPermission(new Permissions(@this.Strategy.Session).Get((Class)@this.Strategy.Class, m.WorkEffort.Complete));
                }
            }
        }

        public static void BaseComplete(this WorkEffort @this, WorkEffortComplete method)
        {
            if (!method.Result.HasValue)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Completed;
                method.Result = true;
            }
        }

        public static void BaseCancel(this WorkEffort @this, WorkEffortCancel cancel) => @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Cancelled;

        public static void BaseReopen(this WorkEffort @this, WorkEffortReopen reopen)
        {
            if (@this.ExistActualStart)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).InProgress;
            }
            else
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Created;
            }
        }

        public static void BaseRevise(this WorkEffort @this, WorkEffortRevise method)
        {
            if (@this.ExistActualStart)
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).InProgress;
            }
            else
            {
                @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Created;
            }
        }

        public static void BaseInvoice(this WorkEffort @this, WorkEffortInvoice method)
        {
            if (!method.Result.HasValue)
            {
                @this.DeriveCanInvoice();

                if (@this.CanInvoice)
                {
                    @this.WorkEffortState = new WorkEffortStates(@this.Strategy.Session).Finished;
                    @this.InvoiceThis();
                }

                method.Result = true;
            }
        }

        private static void VerifyWorkEffortPartyAssignments(this WorkEffort @this, IDerivation derivation)
        {
            var m = @this.Strategy.Session.Database.State().M;

            var existingAssignmentRequired = @this.TakenBy?.RequireExistingWorkEffortPartyAssignment == true;
            var existingAssignments = @this.WorkEffortPartyAssignmentsWhereAssignment.ToArray();

            foreach (ServiceEntry serviceEntry in @this.ServiceEntriesWhereWorkEffort)
            {
                if (serviceEntry is TimeEntry timeEntry)
                {
                    var from = timeEntry.FromDate;
                    var through = timeEntry.ThroughDate;
                    var worker = timeEntry.TimeSheetWhereTimeEntry?.Worker;
                    var facility = timeEntry.WorkEffort.Facility;

                    var matchingAssignment = existingAssignments.FirstOrDefault
                        (a => a.Assignment.Equals(@this)
                        && a.Party.Equals(worker)
                        && ((a.ExistFacility && a.Facility.Equals(facility)) || (!a.ExistFacility && facility == null))
                        && (!a.ExistFromDate || (a.ExistFromDate && (a.FromDate <= from)))
                        && (!a.ExistThroughDate || (a.ExistThroughDate && (a.ThroughDate >= through))));

                    if (matchingAssignment == null)
                    {
                        if (existingAssignmentRequired)
                        {
                            var message = $"No Work Effort Party Assignment matches Worker: {worker}, Facility: {facility}" +
                                $", Work Effort: {@this}, From: {from}, Through {through}";
                            derivation.Validation.AddError(@this, m.WorkEffort.WorkEffortPartyAssignmentsWhereAssignment, message);
                        }
                        else if (worker != null) // Sync a new WorkEffortPartyAssignment
                        {
                            new WorkEffortPartyAssignmentBuilder(@this.Strategy.Session)
                                .WithAssignment(@this)
                                .WithParty(worker)
                                .WithFacility(facility)
                                .Build();
                        }
                    }
                }
            }
        }

        private static void DeriveActualHoursAndDates(this WorkEffort @this)
        {
            @this.ActualHours = 0M;

            foreach (ServiceEntry serviceEntry in @this.ServiceEntriesWhereWorkEffort)
            {
                if (serviceEntry is TimeEntry timeEntry)
                {
                    @this.ActualHours += timeEntry.ActualHours;

                    if (!@this.ExistActualStart)
                    {
                        @this.ActualStart = timeEntry.FromDate;
                    }
                    else if (timeEntry.FromDate < @this.ActualStart)
                    {
                        @this.ActualStart = timeEntry.FromDate;
                    }

                    if (!@this.ExistActualCompletion)
                    {
                        @this.ActualCompletion = timeEntry.ThroughDate;
                    }
                    else if (timeEntry.ThroughDate > @this.ActualCompletion)
                    {
                        @this.ActualCompletion = timeEntry.ThroughDate;
                    }
                }
            }
        }

        private static SalesInvoice InvoiceThis(this WorkEffort @this)
        {
            var salesInvoice = new SalesInvoiceBuilder(@this.Strategy.Session)
                .WithBilledFrom(@this.TakenBy)
                .WithBillToCustomer(@this.Customer)
                .WithBillToContactMechanism(@this.Customer.ExistBillingAddress ? @this.Customer.BillingAddress : @this.Customer.GeneralCorrespondence)
                .WithBillToContactPerson(@this.ContactPerson)
                .WithInvoiceDate(@this.Strategy.Session.Now())
                .WithSalesInvoiceType(new SalesInvoiceTypes(@this.Strategy.Session).SalesInvoice)
                .Build();

            CreateInvoiceItems(@this, salesInvoice);
            foreach (WorkEffort childWorkEffort in @this.Children)
            {
                CreateInvoiceItems(childWorkEffort, salesInvoice);
            }

            return salesInvoice;
        }

        private static void CreateInvoiceItems(this WorkEffort @this, SalesInvoice salesInvoice)
        {
            var session = @this.Strategy.Session;

            var timeEntriesByBillingRate = @this.ServiceEntriesWhereWorkEffort.OfType<TimeEntry>()
                .Where(v => (v.IsBillable && !v.BillableAmountOfTime.HasValue && v.AmountOfTime.HasValue) || v.BillableAmountOfTime.HasValue)
                .GroupBy(v => v.BillingRate)
                .Select(v => v)
                .ToArray();

            foreach (var rateGroup in timeEntriesByBillingRate)
            {
                var timeEntries = rateGroup.ToArray();

                var invoiceItem = new SalesInvoiceItemBuilder(session)
                    .WithInvoiceItemType(new InvoiceItemTypes(session).Time)
                    .WithAssignedUnitPrice(rateGroup.Key)
                    .WithQuantity(timeEntries.Sum(v => v.BillableAmountOfTime ?? v.AmountOfTime ?? 0.0m))
                    .Build();

                salesInvoice.AddSalesInvoiceItem(invoiceItem);

                foreach (var billableEntry in timeEntries)
                {
                    new TimeEntryBillingBuilder(session)
                        .WithTimeEntry(billableEntry)
                        .WithInvoiceItem(invoiceItem)
                        .Build();
                }
            }

            foreach (WorkEffortInventoryAssignment workEffortInventoryAssignment in @this.WorkEffortInventoryAssignmentsWhereAssignment)
            {
                var part = workEffortInventoryAssignment.InventoryItem.Part;

                var quantity = workEffortInventoryAssignment.DerivedBillableQuantity != 0 ? workEffortInventoryAssignment.DerivedBillableQuantity : workEffortInventoryAssignment.Quantity;

                var invoiceItem = new SalesInvoiceItemBuilder(session)
                    .WithInvoiceItemType(new InvoiceItemTypes(session).PartItem)
                    .WithPart(part)
                    .WithAssignedUnitPrice(workEffortInventoryAssignment.UnitSellingPrice)
                    .WithQuantity(quantity)
                    .WithCostOfGoodsSold(workEffortInventoryAssignment.CostOfGoodsSold)
                    .Build();

                salesInvoice.AddSalesInvoiceItem(invoiceItem);

                new WorkEffortBillingBuilder(session)
                    .WithWorkEffort(@this)
                    .WithInvoiceItem(invoiceItem)
                    .Build();
            }
        }

        private static void DeriveCanInvoice(this WorkEffort @this)
        {
            if (!(@this.ExistWorkEffortBillingsWhereWorkEffort && @this.ExistServiceEntriesWhereWorkEffort)
                && @this.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Completed))
            {
                @this.CanInvoice = true;

                if (@this.ExistWorkEffortWhereChild)
                {
                    @this.CanInvoice = false;
                }

                if (@this.CanInvoice)
                {
                    foreach (WorkEffort child in @this.Children)
                    {
                        if (!child.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Completed))
                        {
                            @this.CanInvoice = false;
                            break;
                        }
                    }
                }

                if (@this.CanInvoice)
                {
                    foreach (TimeEntry timeEntry in @this.ServiceEntriesWhereWorkEffort)
                    {
                        if (!timeEntry.ExistThroughDate)
                        {
                            @this.CanInvoice = false;
                            break;
                        }
                    }
                }

                if (@this.ExistWorkEffortAssignmentRatesWhereWorkEffort && !@this.ExistWorkEffortAssignmentRatesWhereWorkEffort)
                {
                    @this.CanInvoice = false;
                }
            }
            else
            {
                @this.CanInvoice = false;
            }
        }

        private static void BaseCalculateTotalRevenue(this WorkEffort @this, WorkEffortCalculateTotalRevenue method)
        {
            if (!method.Result.HasValue)
            {
                @this.TotalLabourRevenue = Math.Round(@this.BillableTimeEntries().Sum(v => v.BillingAmount), 2);
                @this.TotalMaterialRevenue = Math.Round(@this.WorkEffortInventoryAssignmentsWhereAssignment.Where(v => v.DerivedBillableQuantity > 0).Sum(v => v.DerivedBillableQuantity * v.UnitSellingPrice), 2);
                @this.TotalSubContractedRevenue = Math.Round(@this.WorkEffortPurchaseOrderItemAssignmentsWhereAssignment.Sum(v => v.Quantity * v.UnitSellingPrice), 2);
                var totalRevenue = Math.Round(@this.TotalLabourRevenue + @this.TotalMaterialRevenue + @this.TotalSubContractedRevenue, 2);

                method.Result = true;

                @this.GrandTotal = totalRevenue;
                @this.TotalRevenue = @this.Customer.Equals(@this.ExecutedBy) ? 0M : totalRevenue;
            }
        }
    }
}
