//------------------------------------------------------------------------------------------------- 
// <copyright file="PurchaseInvoiceTests.cs" company="Allors bvba">
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
    using Meta;
    using Xunit;
    using Resources;

    
    public class PurchaseInvoiceTests : DomainTest
    {
        [Fact]
        public void GivenPurchaseInvoice_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var builder = new PurchaseInvoiceBuilder(this.DatabaseSession);
            builder.Build();

            Assert.True(this.DatabaseSession.Derive(false).HasErrors);

            this.DatabaseSession.Rollback();

            builder.WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.DatabaseSession).PurchaseInvoice);
            builder.Build();

            Assert.True(this.DatabaseSession.Derive(false).HasErrors);

            this.DatabaseSession.Rollback();

            builder.WithBilledFromParty(new Organisations(this.DatabaseSession).FindBy(M.Organisation.Name, "supplier"));
            builder.Build();

            Assert.False(this.DatabaseSession.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenPurchaseInvoice_WhenDeriving_ThenBilledFromPartyMustBeInSupplierRelationship()
        {
            var supplier2 = new OrganisationBuilder(this.DatabaseSession).WithName("supplier2").WithOrganisationRole(new OrganisationRoles(this.DatabaseSession).Supplier).Build();

            var invoice = new PurchaseInvoiceBuilder(this.DatabaseSession)
                .WithInvoiceNumber("1")
                .WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.DatabaseSession).PurchaseInvoice)
                .WithBilledFromParty(supplier2)
                .Build();

            Assert.Equal(ErrorMessages.PartyIsNotASupplier, this.DatabaseSession.Derive(false).Errors[0].Message);

            new SupplierRelationshipBuilder(this.DatabaseSession).WithSupplier(supplier2).Build();

            Assert.False(this.DatabaseSession.Derive(false).HasErrors);
        }

        [Fact]
        public void GivenPurchaseInvoice_WhenGettingInvoiceNumberWithoutFormat_ThenInvoiceNumberShouldBeReturned()
        {
            var belgium = new Countries(this.DatabaseSession).CountryByIsoCode["BE"];
            var euro = belgium.Currency;

            var bank = new BankBuilder(this.DatabaseSession).WithCountry(belgium).WithName("ING Belgi�").WithBic("BBRUBEBB").Build();

            var ownBankAccount = new OwnBankAccountBuilder(this.DatabaseSession)
                .WithDescription("BE23 3300 6167 6391")
                .WithBankAccount(new BankAccountBuilder(this.DatabaseSession).WithBank(bank).WithCurrency(euro).WithIban("BE23 3300 6167 6391").WithNameOnAccount("Koen").Build())
                .Build();

            var internalOrganisation = new InternalOrganisationBuilder(this.DatabaseSession)
                .WithName("org")
                .WithDefaultPaymentMethod(ownBankAccount)
                .Build();

            var invoice1 = new PurchaseInvoiceBuilder(this.DatabaseSession)
                .WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.DatabaseSession).PurchaseInvoice)
                .Build();

            Assert.Equal("1", invoice1.InvoiceNumber);

            var invoice2 = new PurchaseInvoiceBuilder(this.DatabaseSession)
                .WithPurchaseInvoiceType(new PurchaseInvoiceTypes(this.DatabaseSession).PurchaseInvoice)
                .Build();

            Assert.Equal("2", invoice2.InvoiceNumber);
        }
    }
}