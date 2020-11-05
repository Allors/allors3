// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Meta;

    public class PersonDeniedPermissionDerivation : DomainDerivation
    {
        public PersonDeniedPermissionDerivation(M m) : base(m, new Guid("a88d6239-030c-47a2-8995-e9cbb83d20c7")) =>
            this.Patterns = new Pattern[]
        {
            new ChangedPattern(this.M.TimeSheet.Worker) { Steps = new IPropertyType[] {m.TimeSheet.Worker }, OfType = m.Person.Class },
            new ChangedPattern(this.M.ExternalAccountingTransaction.FromParty) { Steps = new IPropertyType[] {m.ExternalAccountingTransaction.FromParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.ExternalAccountingTransaction.ToParty) { Steps = new IPropertyType[] {m.ExternalAccountingTransaction.ToParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Shipment.ShipFromParty) { Steps = new IPropertyType[] {m.Shipment.ShipFromParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Shipment.ShipToParty) { Steps = new IPropertyType[] {m.Shipment.ShipToParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Payment.Receiver) { Steps = new IPropertyType[] {m.Payment.Receiver }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Payment.Sender) { Steps = new IPropertyType[] {m.Payment.Sender }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Engagement.BillToParty) { Steps = new IPropertyType[] {m.Engagement.BillToParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Engagement.PlacingParty) { Steps = new IPropertyType[] {m.Engagement.PlacingParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.UnifiedGood.ManufacturedBy) { Steps = new IPropertyType[] {m.UnifiedGood.ManufacturedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.UnifiedGood.SuppliedBy) { Steps = new IPropertyType[] {m.UnifiedGood.SuppliedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PartyFixedAssetAssignment.Party) { Steps = new IPropertyType[] {m.PartyFixedAssetAssignment.Party }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PickList.ShipToParty) { Steps = new IPropertyType[] {m.PickList.ShipToParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Quote.Receiver) { Steps = new IPropertyType[] {m.Quote.Receiver }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseInvoice.BilledFrom) { Steps = new IPropertyType[] {m.PurchaseInvoice.BilledFrom }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseInvoice.ShipToCustomer) { Steps = new IPropertyType[] {m.PurchaseInvoice.ShipToCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseInvoice.BillToEndCustomer) { Steps = new IPropertyType[] {m.PurchaseInvoice.BillToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseInvoice.ShipToEndCustomer) { Steps = new IPropertyType[] {m.PurchaseInvoice.ShipToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseOrder.TakenViaSupplier) { Steps = new IPropertyType[] {m.PurchaseOrder.TakenViaSupplier }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PurchaseOrder.TakenViaSubcontractor) { Steps = new IPropertyType[] {m.PurchaseOrder.TakenViaSubcontractor }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Request.Originator) { Steps = new IPropertyType[] {m.Request.Originator }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Requirement.Authorizer) { Steps = new IPropertyType[] {m.Requirement.Authorizer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Requirement.NeededFor) { Steps = new IPropertyType[] {m.Requirement.NeededFor }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Requirement.Originator) { Steps = new IPropertyType[] {m.Requirement.Originator }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Requirement.ServicedBy) { Steps = new IPropertyType[] {m.Requirement.ServicedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesInvoice.BillToCustomer) { Steps = new IPropertyType[] {m.SalesInvoice.BillToCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesInvoice.BillToEndCustomer) { Steps = new IPropertyType[] {m.SalesInvoice.BillToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesInvoice.ShipToCustomer) { Steps = new IPropertyType[] {m.SalesInvoice.ShipToCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesInvoice.ShipToEndCustomer) { Steps = new IPropertyType[] {m.SalesInvoice.ShipToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrder.BillToCustomer) { Steps = new IPropertyType[] {m.SalesOrder.BillToCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrder.BillToEndCustomer) { Steps = new IPropertyType[] {m.SalesOrder.BillToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrder.ShipToCustomer) { Steps = new IPropertyType[] {m.SalesOrder.ShipToCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrder.ShipToEndCustomer) { Steps = new IPropertyType[] {m.SalesOrder.ShipToEndCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrder.PlacingCustomer) { Steps = new IPropertyType[] {m.SalesOrder.PlacingCustomer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SalesOrderItem.AssignedShipToParty) { Steps = new IPropertyType[] {m.SalesOrderItem.AssignedShipToParty }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SerialisedItem.SuppliedBy) { Steps = new IPropertyType[] {m.SerialisedItem.SuppliedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SerialisedItem.OwnedBy) { Steps = new IPropertyType[] {m.SerialisedItem.OwnedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.SerialisedItem.RentedBy) { Steps = new IPropertyType[] {m.SerialisedItem.RentedBy }, OfType = m.Person.Class },
            new ChangedPattern(this.M.WorkEffort.Customer) { Steps = new IPropertyType[] {m.WorkEffort.Customer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.WorkEffortPartyAssignment.Party) { Steps = new IPropertyType[] {m.WorkEffortPartyAssignment.Party }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Cash.PersonResponsible) { Steps = new IPropertyType[] {m.Cash.PersonResponsible }, OfType = m.Person.Class },
            new ChangedPattern(this.M.CommunicationEvent.Owner) { Steps = new IPropertyType[] {m.CommunicationEvent.Owner }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EngagementItem.CurrentAssignedProfessional) { Steps = new IPropertyType[] {m.EngagementItem.CurrentAssignedProfessional }, OfType = m.Person.Class },
            new ChangedPattern(this.M.Employment.Employee) { Steps = new IPropertyType[] {m.Employment.Employee }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EngineeringChange.Authorizer) { Steps = new IPropertyType[] {m.EngineeringChange.Authorizer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EngineeringChange.Designer) { Steps = new IPropertyType[] {m.EngineeringChange.Designer }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EngineeringChange.Requestor) { Steps = new IPropertyType[] {m.EngineeringChange.Requestor }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EngineeringChange.Tester) { Steps = new IPropertyType[] {m.EngineeringChange.Tester }, OfType = m.Person.Class },
            new ChangedPattern(this.M.EventRegistration.Person) { Steps = new IPropertyType[] {m.EventRegistration.Person }, OfType = m.Person.Class },
            new ChangedPattern(this.M.OwnCreditCard.Owner) { Steps = new IPropertyType[] {m.OwnCreditCard.Owner }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PerformanceNote.Employee) { Steps = new IPropertyType[] {m.PerformanceNote.Employee }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PerformanceNote.GivenByManager) { Steps = new IPropertyType[] {m.PerformanceNote.GivenByManager }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PerformanceReview.Employee) { Steps = new IPropertyType[] {m.PerformanceReview.Employee }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PerformanceReview.Manager) { Steps = new IPropertyType[] {m.PerformanceReview.Manager }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PickList.Picker) { Steps = new IPropertyType[] {m.PickList.Picker }, OfType = m.Person.Class },
            new ChangedPattern(this.M.PositionFulfillment.Person) { Steps = new IPropertyType[] {m.PositionFulfillment.Person }, OfType = m.Person.Class },
            new ChangedPattern(this.M.ProfessionalAssignment.Professional) { Steps = new IPropertyType[] {m.ProfessionalAssignment.Professional }, OfType = m.Person.Class },
        };
        
        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var session = cycle.Session;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<Person>())
            {
                var deletePermission = new Permissions(@this.Strategy.Session).Get(@this.Meta.ObjectType, @this.Meta.Delete);
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
