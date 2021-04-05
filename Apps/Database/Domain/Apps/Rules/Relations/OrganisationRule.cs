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

    public class OrganisationRule : Rule
    {
        public OrganisationRule(MetaPopulation m) : base(m, new Guid("0379B923-210D-46DD-9D18-9D7BF5ED6FEA")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.Organisation, m.Organisation.Name),
                new RolePattern(m.Organisation, m.Organisation.UniqueId),
                new RolePattern(m.Organisation, m.Organisation.DerivationTrigger),
                new AssociationPattern(m.Employment.Employer),
                new RolePattern(m.Employment, m.Employment.FromDate) {Steps = new IPropertyType[]{ m.Employment.Employer}, OfType = m.Organisation},
                new RolePattern(m.Employment, m.Employment.ThroughDate) {Steps = new IPropertyType[]{ m.Employment.Employer}, OfType = m.Organisation},
                new AssociationPattern(m.CustomerRelationship.InternalOrganisation),
                new RolePattern(m.CustomerRelationship, m.CustomerRelationship.FromDate) {Steps = new IPropertyType[]{ m.CustomerRelationship.InternalOrganisation} },
                new RolePattern(m.CustomerRelationship, m.CustomerRelationship.ThroughDate) {Steps = new IPropertyType[]{ m.CustomerRelationship.InternalOrganisation} },
                new AssociationPattern(m.SupplierRelationship.InternalOrganisation),
                new RolePattern(m.SupplierRelationship, m.SupplierRelationship.FromDate) {Steps = new IPropertyType[]{ m.SupplierRelationship.InternalOrganisation} },
                new RolePattern(m.SupplierRelationship, m.SupplierRelationship.ThroughDate) {Steps = new IPropertyType[]{ m.SupplierRelationship.InternalOrganisation} },
                new AssociationPattern(m.OrganisationContactRelationship.Organisation),
                new RolePattern(m.OrganisationContactRelationship, m.OrganisationContactRelationship.FromDate) {Steps = new IPropertyType[]{ m.OrganisationContactRelationship.Organisation} },
                new RolePattern(m.OrganisationContactRelationship, m.OrganisationContactRelationship.ThroughDate) {Steps = new IPropertyType[]{ m.OrganisationContactRelationship.Organisation} },
                new AssociationPattern(m.SubContractorRelationship.Contractor),
                new RolePattern(m.SubContractorRelationship, m.SubContractorRelationship.FromDate) {Steps = new IPropertyType[]{ m.SubContractorRelationship.Contractor } },
                new RolePattern(m.SubContractorRelationship, m.SubContractorRelationship.ThroughDate) {Steps = new IPropertyType[]{ m.SubContractorRelationship.Contractor } },
                new RolePattern(m.Organisation, m.Organisation.PartyContactMechanisms),
                new RolePattern(m.PartyContactMechanism, m.PartyContactMechanism.FromDate) {Steps = new IPropertyType[]{ m.PartyContactMechanism.PartyWherePartyContactMechanism }, OfType = m.Organisation },
                new RolePattern(m.PartyContactMechanism, m.PartyContactMechanism.ThroughDate) {Steps = new IPropertyType[]{ m.PartyContactMechanism.PartyWherePartyContactMechanism }, OfType = m.Organisation },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var transaction = cycle.Transaction;

            foreach (var @this in matches.Cast<Organisation>())
            {
                var now = transaction.Now();

                transaction.Prefetch(@this.PrefetchPolicy);

                @this.PartyName = @this.Name;

                if (!@this.ExistContactsUserGroup)
                {
                    var customerContactGroupName = $"Customer contacts at {@this.Name} ({@this.UniqueId})";
                    @this.ContactsUserGroup = new UserGroupBuilder(@this.Strategy.Transaction).WithName(customerContactGroupName).Build();
                }

                @this.DeriveRelationships();

                var partyContactMechanisms = @this.PartyContactMechanisms?.ToArray();
                foreach (OrganisationContactRelationship organisationContactRelationship in @this.OrganisationContactRelationshipsWhereOrganisation)
                {
                    organisationContactRelationship.Contact?.Sync(partyContactMechanisms);
                }
            }
        }
    }
}
