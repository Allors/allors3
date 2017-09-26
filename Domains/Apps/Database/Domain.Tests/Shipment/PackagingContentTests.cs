//------------------------------------------------------------------------------------------------- 
// <copyright file="PackagingContentTests.cs" company="Allors bvba">
// Copyright 2002-2009 Allors bvba.
// 
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// 
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>Defines the MediaTests type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Allors.Domain
{
    using System;
    using Meta;
    using Xunit;

    
    public class PackagingContentTests : DomainTest
    {
        [Fact]
        public void GivenPackingingContent_WhenDeriving_ThenAssertQuantityPackedIsNotGreaterThanQuantityShipped()
        {
            var mechelen = new CityBuilder(this.DatabaseSession).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.DatabaseSession).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.DatabaseSession)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.DatabaseSession).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var customer = new PersonBuilder(this.DatabaseSession).WithLastName("customer").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.DatabaseSession).Customer).Build();
            var internalOrganisation = Singleton.Instance(this.DatabaseSession);
            

            var vatRate21 = new VatRateBuilder(this.DatabaseSession).WithRate(21).Build();

            var good1 = new GoodBuilder(this.DatabaseSession)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithLocalisedName(new LocalisedTextBuilder(this.DatabaseSession).WithText("good1").WithLocale(Singleton.Instance(this.DatabaseSession).DefaultLocale).Build())
                .WithInventoryItemKind(new InventoryItemKinds(this.DatabaseSession).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.DatabaseSession).Piece)
                .Build();

            var good2 = new GoodBuilder(this.DatabaseSession)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithLocalisedName(new LocalisedTextBuilder(this.DatabaseSession).WithText("good2").WithLocale(Singleton.Instance(this.DatabaseSession).DefaultLocale).Build())
                .WithInventoryItemKind(new InventoryItemKinds(this.DatabaseSession).NonSerialised)
                .WithUnitOfMeasure(new UnitsOfMeasure(this.DatabaseSession).Piece)
                .Build();

            var good1inventoryItem = new NonSerialisedInventoryItemBuilder(this.DatabaseSession).WithGood(good1).Build();
            good1inventoryItem.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.DatabaseSession).WithQuantity(100).WithReason(new VarianceReasons(this.DatabaseSession).Ruined).Build());

            this.DatabaseSession.Derive();

            var good2inventoryItem = new NonSerialisedInventoryItemBuilder(this.DatabaseSession).WithGood(good2).Build();
            good2inventoryItem.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.DatabaseSession).WithQuantity(100).WithReason(new VarianceReasons(this.DatabaseSession).Ruined).Build());

            this.DatabaseSession.Derive();

            var order = new SalesOrderBuilder(this.DatabaseSession)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order.AddSalesOrderItem(item1);
            order.AddSalesOrderItem(item2);
            order.AddSalesOrderItem(item3);

            this.DatabaseSession.Derive();

            order.Confirm();

            this.DatabaseSession.Derive();

            var shipment = (CustomerShipment)mechelenAddress.ShipmentsWhereShipToAddress[0];

            var package = new ShipmentPackageBuilder(this.DatabaseSession).Build();
            package.AddPackagingContent(new PackagingContentBuilder(this.DatabaseSession)
                                            .WithShipmentItem(shipment.ShipmentItems[0])
                                            .WithQuantity(shipment.ShipmentItems[0].Quantity + 1)
                                            .Build());

            Assert.True(this.DatabaseSession.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenPackingingContent_WhenDerived_ThenShipmentItemsQuantityPackedIsSet()
        {
            var mechelen = new CityBuilder(this.DatabaseSession).WithName("Mechelen").Build();
            var mechelenAddress = new PostalAddressBuilder(this.DatabaseSession).WithGeographicBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var shipToMechelen = new PartyContactMechanismBuilder(this.DatabaseSession)
                .WithContactMechanism(mechelenAddress)
                .WithContactPurpose(new ContactMechanismPurposes(this.DatabaseSession).ShippingAddress)
                .WithUseAsDefault(true)
                .Build();

            var customer = new PersonBuilder(this.DatabaseSession).WithLastName("customer").WithPartyContactMechanism(shipToMechelen).WithPersonRole(new PersonRoles(this.DatabaseSession).Customer).Build();
            var internalOrganisation = Singleton.Instance(this.DatabaseSession);
            

            var vatRate21 = new VatRateBuilder(this.DatabaseSession).WithRate(21).Build();
            var good1 = new GoodBuilder(this.DatabaseSession)
                .WithSku("10101")
                .WithVatRate(vatRate21)
                .WithLocalisedName(new LocalisedTextBuilder(this.DatabaseSession).WithText("good1").WithLocale(Singleton.Instance(this.DatabaseSession).DefaultLocale).Build())
                .WithUnitOfMeasure(new UnitsOfMeasure(this.DatabaseSession).Piece)
                .WithInventoryItemKind(new InventoryItemKinds(this.DatabaseSession).NonSerialised)
                .Build();

            var good2 = new GoodBuilder(this.DatabaseSession)
                .WithSku("10102")
                .WithVatRate(vatRate21)
                .WithLocalisedName(new LocalisedTextBuilder(this.DatabaseSession).WithText("good2").WithLocale(Singleton.Instance(this.DatabaseSession).DefaultLocale).Build())
                .WithUnitOfMeasure(new UnitsOfMeasure(this.DatabaseSession).Piece)
                .WithInventoryItemKind(new InventoryItemKinds(this.DatabaseSession).NonSerialised)
                .Build();

            var good1inventoryItem = new NonSerialisedInventoryItemBuilder(this.DatabaseSession).WithGood(good1).Build();
            good1inventoryItem.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.DatabaseSession).WithQuantity(100).WithReason(new VarianceReasons(this.DatabaseSession).Ruined).Build());

            this.DatabaseSession.Derive();

            var good2inventoryItem = new NonSerialisedInventoryItemBuilder(this.DatabaseSession).WithGood(good2).Build();
            good2inventoryItem.AddInventoryItemVariance(new InventoryItemVarianceBuilder(this.DatabaseSession).WithQuantity(100).WithReason(new VarianceReasons(this.DatabaseSession).Ruined).Build());

            this.DatabaseSession.Derive();

            var order = new SalesOrderBuilder(this.DatabaseSession)
                .WithBillToCustomer(customer)
                .WithShipToCustomer(customer)
                .Build();

            var item1 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good1).WithQuantityOrdered(1).WithActualUnitPrice(15).Build();
            var item2 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good1).WithQuantityOrdered(2).WithActualUnitPrice(15).Build();
            var item3 = new SalesOrderItemBuilder(this.DatabaseSession).WithProduct(good2).WithQuantityOrdered(5).WithActualUnitPrice(15).Build();
            order.AddSalesOrderItem(item1);
            order.AddSalesOrderItem(item2);
            order.AddSalesOrderItem(item3);

            this.DatabaseSession.Derive();

            order.Confirm();

            this.DatabaseSession.Derive();

            var shipment = (CustomerShipment)mechelenAddress.ShipmentsWhereShipToAddress[0];
            var package = new ShipmentPackageBuilder(this.DatabaseSession).Build();
            foreach (ShipmentItem shipmentItem in shipment.ShipmentItems)
            {
                package.AddPackagingContent(new PackagingContentBuilder(this.DatabaseSession).WithShipmentItem(shipmentItem).WithQuantity(shipmentItem.Quantity).Build());
            }

            this.DatabaseSession.Derive();

            foreach (ShipmentItem shipmentItem in shipment.ShipmentItems)
            {
                Assert.Equal(shipmentItem.QuantityShipped, shipmentItem.Quantity);
            }
        }
    }
}