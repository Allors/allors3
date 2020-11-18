// <copyright file="PartyTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines the PersonTests type.
// </summary>

namespace Allors.Database.Domain.Tests
{
    using System.Linq;
    using Xunit;

    public class PartyTests : DomainTest, IClassFixture<Fixture>
    {
        public PartyTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenPartyWithOpenOrders_WhenDeriving_ThenOpenOrderAmountIsUpdated()
        {
            var store = this.Session.Extent<Store>().First;
            store.IsImmediatelyPicked = false;

            var organisation = new OrganisationBuilder(this.Session).WithName("customer").Build();
            var customerRelationship = new CustomerRelationshipBuilder(this.Session).WithCustomer(organisation).Build();

            this.Session.Derive();

            var partyFinancial = organisation.PartyFinancialRelationshipsWhereFinancialParty.First(v => Equals(v.InternalOrganisation, customerRelationship.InternalOrganisation));

            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();

            var postalAddress = new PostalAddressBuilder(this.Session)
                  .WithAddress1("Kleine Nieuwedijkstraat 2")
                  .WithPostalAddressBoundary(mechelen)
                  .Build();

            var good = new Goods(this.Session).FindBy(this.M.Good.Name, "good1");

            this.Session.Derive();

            var salesOrder1 = new SalesOrderBuilder(this.Session).WithBillToCustomer(organisation).WithShipToAddress(postalAddress).WithComment("salesorder1").Build();
            var orderItem1 = new SalesOrderItemBuilder(this.Session)
                .WithProduct(good)
                .WithQuantityOrdered(10)
                .WithAssignedUnitPrice(10)
                .Build();
            salesOrder1.AddSalesOrderItem(orderItem1);

            this.Session.Derive();

            var salesOrder2 = new SalesOrderBuilder(this.Session).WithBillToCustomer(organisation).WithShipToAddress(postalAddress).WithComment("salesorder2").Build();
            var orderItem2 = new SalesOrderItemBuilder(this.Session)
                .WithProduct(good)
                .WithQuantityOrdered(10)
                .WithAssignedUnitPrice(10)
                .Build();
            salesOrder2.AddSalesOrderItem(orderItem2);

            this.Session.Derive();

            var salesOrder3 = new SalesOrderBuilder(this.Session).WithBillToCustomer(organisation).WithShipToAddress(postalAddress).WithComment("salesorder3").Build();
            var orderItem3 = new SalesOrderItemBuilder(this.Session)
                .WithProduct(good)
                .WithQuantityOrdered(10)
                .WithAssignedUnitPrice(10)
                .Build();
            salesOrder3.AddSalesOrderItem(orderItem3);
            salesOrder3.Cancel();

            this.Session.Derive();

            Assert.Equal(200M, partyFinancial.OpenOrderAmount);
        }
    }
}
