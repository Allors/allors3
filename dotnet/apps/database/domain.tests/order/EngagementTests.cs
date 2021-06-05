// <copyright file="EngagementTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class EngagementTests : DomainTest, IClassFixture<Fixture>
    {
        public EngagementTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenEngagement_WhenDeriving_ThenDescriptionIsRequired()
        {
            var mechelen = new CityBuilder(this.Transaction).WithName("Mechelen").Build();
            var billToContactMechanism = new PostalAddressBuilder(this.Transaction).WithPostalAddressBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var partyContactMechanism = new PartyContactMechanismBuilder(this.Transaction)
                .WithContactMechanism(billToContactMechanism)
                .WithContactPurpose(new ContactMechanismPurposes(this.Transaction).BillingAddress)
                .WithUseAsDefault(true)
                .Build();

            var customer = new OrganisationBuilder(this.Transaction).WithName("customer").WithPartyContactMechanism(partyContactMechanism).Build();

            this.Transaction.Derive();
            this.Transaction.Commit();

            var builder = new EngagementBuilder(this.Transaction);
            var customEngagementItem = builder.Build();

            Assert.True(this.Transaction.Derive(false).HasErrors);

            this.Transaction.Rollback();

            builder.WithDescription("Engagement");
            customEngagementItem = builder.Build();

            Assert.True(this.Transaction.Derive(false).HasErrors);

            this.Transaction.Rollback();

            builder.WithBillToParty(customer);
            customEngagementItem = builder.Build();

            Assert.False(this.Transaction.Derive(false).HasErrors);
        }
    }

    public class EngagementRuleTests : DomainTest, IClassFixture<Fixture>
    {
        public EngagementRuleTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void ChangedBillToPartyDeriveBillToContactMechanism()
        {
            var billToParty = this.InternalOrganisation.ActiveCustomers[0];

            var partyContactMechanism = new PartyContactMechanismBuilder(this.Transaction)
                .WithUseAsDefault(true)
                .WithContactMechanism(new PostalAddressBuilder(this.Transaction).Build())
                .WithContactPurpose(new ContactMechanismPurposes(this.Transaction).BillingAddress)
                .Build();

            billToParty.AddPartyContactMechanism(partyContactMechanism);
            this.Transaction.Derive(false);

            var engagement = new EngagementBuilder(this.Transaction).Build();
            this.Transaction.Derive(false);

            engagement.BillToParty = billToParty;
            this.Transaction.Derive(false);

            Assert.Equal(partyContactMechanism.ContactMechanism, engagement.BillToContactMechanism);
        }

        [Fact]
        public void ChangedPlacingPartyDerivePlacingContactMechanism()
        {
            var placingParty = this.InternalOrganisation.ActiveCustomers[0];

            var partyContactMechanism = new PartyContactMechanismBuilder(this.Transaction)
                .WithUseAsDefault(true)
                .WithContactMechanism(new PostalAddressBuilder(this.Transaction).Build())
                .WithContactPurpose(new ContactMechanismPurposes(this.Transaction).OrderAddress)
                .Build();

            placingParty.AddPartyContactMechanism(partyContactMechanism);
            this.Transaction.Derive(false);

            var engagement = new EngagementBuilder(this.Transaction).Build();
            this.Transaction.Derive(false);

            engagement.PlacingParty = placingParty;
            this.Transaction.Derive(false);

            Assert.Equal(partyContactMechanism.ContactMechanism, engagement.PlacingContactMechanism);
        }
    }
}