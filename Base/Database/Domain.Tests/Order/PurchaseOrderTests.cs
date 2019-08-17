//-------------------------------------------------------------------------------------------------
// <copyright file="PurchaseOrderTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Allors.Domain
{
    using Resources;
    using Allors.Meta;
    using Xunit;

    public class PurchaseOrderTests : DomainTest
    {
        [Fact]
        public void GivenPurchaseOrderBuilder_WhenBuild_ThenPostBuildRelationsMustExist()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").Build();
            var internalOrganisation = this.InternalOrganisation;
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var order = new PurchaseOrderBuilder(this.Session).WithTakenViaSupplier(supplier).Build();

            this.Session.Derive();

            Assert.Equal(new PurchaseOrderStates(this.Session).Created, order.PurchaseOrderState);
            Assert.Equal(this.Session.Now().Date, order.OrderDate.Date);
            Assert.Equal(this.Session.Now().Date, order.EntryDate.Date);
            Assert.Equal(order.PreviousTakenViaSupplier, order.TakenViaSupplier);
        }

        [Fact]
        public void GivenOrder_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            var internalOrganisation = this.InternalOrganisation;
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            ContactMechanism takenViaContactMechanism = new PostalAddressBuilder(this.Session).WithPostalAddressBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var supplierContactMechanism = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(takenViaContactMechanism)
                .WithUseAsDefault(true)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).OrderAddress)
                .Build();
            supplier.AddPartyContactMechanism(supplierContactMechanism);

            this.Session.Derive();
            this.Session.Commit();

            var builder = new PurchaseOrderBuilder(this.Session);
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            this.Session.Rollback();

            builder.WithTakenViaSupplier(supplier);
            builder.Build();

            Assert.False(this.Session.Derive(false).HasErrors);

            builder.WithTakenViaContactMechanism(takenViaContactMechanism);
            builder.Build();

            Assert.False(this.Session.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenPurchaseOrder_WhenDeriving_ThenTakenViaSupplierMustBeInSupplierRelationship()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            var internalOrganisation = this.InternalOrganisation;

            new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            var expectedError = ErrorMessages.PartyIsNotASupplier;
            Assert.Equal(expectedError, this.Session.Derive(false).Errors[0].Message);

            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            Assert.False(this.Session.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenOrder_WhenDeriving_ThenLocaleMustExist()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var mechelen = new CityBuilder(this.Session).WithName("Mechelen").Build();
            ContactMechanism takenViaContactMechanism = new PostalAddressBuilder(this.Session).WithPostalAddressBoundary(mechelen).WithAddress1("Haverwerf 15").Build();
            var supplierContactMechanism = new PartyContactMechanismBuilder(this.Session)
                .WithContactMechanism(takenViaContactMechanism)
                .WithUseAsDefault(true)
                .WithContactPurpose(new ContactMechanismPurposes(this.Session).OrderAddress)
                .Build();
            supplier.AddPartyContactMechanism(supplierContactMechanism);

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            this.Session.Derive();

            Assert.Equal(this.Session.GetSingleton().DefaultLocale, order.Locale);
        }

        [Fact]
        public void GivenPurchaseOrder_WhenGettingOrderNumberWithoutFormat_ThenOrderNumberShouldBeReturned()
        {
            var internalOrganisation = this.InternalOrganisation;
            internalOrganisation.RemovePurchaseOrderNumberPrefix();

            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            this.Session.Derive();

            var order1 = new PurchaseOrderBuilder(this.Session).WithTakenViaSupplier(supplier).Build();

            this.Session.Derive();

            Assert.Equal("1", order1.OrderNumber);

            var order2 = new PurchaseOrderBuilder(this.Session).WithTakenViaSupplier(supplier).Build();

            this.Session.Derive();

            Assert.Equal("2", order2.OrderNumber);
        }

        [Fact]
        public void GivenPurchaseOrder_WhenGettingOrderNumberWithFormat_ThenFormattedOrderNumberShouldBeReturned()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("supplier").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            this.Session.Derive();

            var internalOrganisation = this.InternalOrganisation;
            internalOrganisation.PurchaseOrderNumberPrefix = "the format is ";

            var order1 = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            this.Session.Derive();

            Assert.Equal("the format is 1", order1.OrderNumber);

            var order2 = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            this.Session.Derive();

            Assert.Equal("the format is 2", order2.OrderNumber);
        }

        [Fact]
        public void GivenPurchaseOrder_WhenConfirming_ThenAllValidItemsAreInConfirmedState()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var part = new NonUnifiedPartBuilder(this.Session)
                .WithProductIdentification(new PartNumberBuilder(this.Session)
                    .WithIdentification("1")
                    .WithProductIdentificationType(new ProductIdentificationTypes(this.Session).Part).Build())
                .Build();

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .WithVatRegime(new VatRegimes(this.Session).Exempt)
                .Build();

            var item1 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(1).Build();
            var item2 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(2).Build();
            var item3 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(3).Build();
            var item4 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(4).Build();
            order.AddPurchaseOrderItem(item1);
            order.AddPurchaseOrderItem(item2);
            order.AddPurchaseOrderItem(item3);
            order.AddPurchaseOrderItem(item4);

            this.Session.Derive();

            order.Confirm();

            this.Session.Derive();

            item4.Cancel();

            this.Session.Derive();

            Assert.Equal(3, order.ValidOrderItems.Count);
            Assert.Contains(item1, order.ValidOrderItems);
            Assert.Contains(item2, order.ValidOrderItems);
            Assert.Contains(item3, order.ValidOrderItems);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item1.PurchaseOrderItemState);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item2.PurchaseOrderItemState);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item3.PurchaseOrderItemState);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).Cancelled, item4.PurchaseOrderItemState);
        }

        [Fact]
        public void GivenPurchaseOrder_WhenOrdering_ThenAllValidItemsAreInInProcessState()
        {
            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var part = new NonUnifiedPartBuilder(this.Session)
                .WithProductIdentification(new PartNumberBuilder(this.Session)
                    .WithIdentification("1")
                    .WithProductIdentificationType(new ProductIdentificationTypes(this.Session).Part).Build())
                .Build();

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .WithVatRegime(new VatRegimes(this.Session).Exempt)
                .Build();

            var item1 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(1).Build();
            var item2 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(2).Build();
            var item3 = new PurchaseOrderItemBuilder(this.Session).WithPart(part).WithQuantityOrdered(3).Build();
            order.AddPurchaseOrderItem(item1);
            order.AddPurchaseOrderItem(item2);
            order.AddPurchaseOrderItem(item3);

            this.Session.Derive();

            order.Confirm();

            this.Session.Derive();

            Assert.Equal(3, order.ValidOrderItems.Count);
            Assert.Contains(item1, order.ValidOrderItems);
            Assert.Contains(item2, order.ValidOrderItems);
            Assert.Contains(item3, order.ValidOrderItems);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item1.PurchaseOrderItemState);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item2.PurchaseOrderItemState);
            Assert.Equal(new PurchaseOrderItemStates(this.Session).InProcess, item3.PurchaseOrderItemState);
        }
    }

    public class PurchaseOrderSecurityTests : DomainTest
    {
        public override Config Config => new Config { SetupSecurity = true };

        [Fact]
        public void GivenPurchaseOrder_WhenObjectStateIsApproved_ThenCheckTransitions()
        {
            this.SetIdentity(Users.AdministratorUserName);

            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            var internalOrganisation = this.InternalOrganisation;
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            order.Confirm();

            this.Session.Derive();

            order.Approve();

            this.Session.Derive();

            Assert.Equal(new PurchaseOrderStates(this.Session).InProcess, order.PurchaseOrderState);
            var acl = new AccessControlList(order, this.Session.GetUser());
            Assert.False(acl.CanExecute(M.PurchaseOrder.Approve));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Reject));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Continue));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Confirm));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Reopen));
            Assert.True(acl.CanExecute(M.PurchaseOrder.QuickReceive));
        }

        [Fact]
        public void GivenPurchaseOrder_WhenObjectStateIsInProcess_ThenCheckTransitions()
        {
            this.SetIdentity(Users.AdministratorUserName);

            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            var internalOrganisation = this.InternalOrganisation;
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            order.Confirm();

            this.Session.Derive();

            Assert.Equal(new PurchaseOrderStates(this.Session).InProcess, order.PurchaseOrderState);
            var acl = new AccessControlList(order, this.Session.GetUser());
            Assert.True(acl.CanExecute(M.PurchaseOrder.Cancel));
            Assert.True(acl.CanExecute(M.PurchaseOrder.Hold));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Confirm));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Reject));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Approve));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Continue));
        }

        [Fact]
        public void GivenPurchaseOrder_WhenObjectStateIsOnHold_ThenCheckTransitions()
        {
            this.SetIdentity(Users.AdministratorUserName);

            var supplier = new OrganisationBuilder(this.Session).WithName("customer2").Build();
            new SupplierRelationshipBuilder(this.Session).WithSupplier(supplier).Build();

            var order = new PurchaseOrderBuilder(this.Session)
                .WithTakenViaSupplier(supplier)
                .Build();

            order.Confirm();

            this.Session.Derive();

            order.Hold();

            this.Session.Derive();

            Assert.Equal(new PurchaseOrderStates(this.Session).OnHold, order.PurchaseOrderState);
            var acl = new AccessControlList(order, this.Session.GetUser());
            Assert.False(acl.CanExecute(M.PurchaseOrder.Approve));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Hold));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Confirm));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Reopen));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Send));
            Assert.False(acl.CanExecute(M.PurchaseOrder.QuickReceive));
            Assert.False(acl.CanExecute(M.PurchaseOrder.Invoice));

            Assert.True(acl.CanExecute(M.PurchaseOrder.Reject));
            Assert.True(acl.CanExecute(M.PurchaseOrder.Continue));
            Assert.True(acl.CanExecute(M.PurchaseOrder.Cancel));
        }
    }
}
