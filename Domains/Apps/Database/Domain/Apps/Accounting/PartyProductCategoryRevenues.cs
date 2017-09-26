// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartyProductCategoryRevenues.cs" company="Allors bvba">
//   Copyright 2002-2012 Allors bvba.
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using Meta;

    public partial class PartyProductCategoryRevenues
    {
        public static void AppsOnDeriveRevenues(ISession session)
        {
            var partyProductCategoryRevenuesByPeriodByProductCategoryByParty = new Dictionary<Party, Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>>>();

            var partyProductCategoryRevenues = session.Extent<PartyProductCategoryRevenue>();

            foreach (PartyProductCategoryRevenue partyProductCategoryRevenue in partyProductCategoryRevenues)
            {
                partyProductCategoryRevenue.Revenue = 0;
                partyProductCategoryRevenue.Quantity = 0;
                var date = DateTimeFactory.CreateDate(partyProductCategoryRevenue.Year, partyProductCategoryRevenue.Month, 01);

                Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>> partyProductCategoryRevenuesByPeriodByproductCategory;
                if (!partyProductCategoryRevenuesByPeriodByProductCategoryByParty.TryGetValue(partyProductCategoryRevenue.Party, out partyProductCategoryRevenuesByPeriodByproductCategory))
                {
                    partyProductCategoryRevenuesByPeriodByproductCategory = new Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>>();
                    partyProductCategoryRevenuesByPeriodByProductCategoryByParty[partyProductCategoryRevenue.Party] = partyProductCategoryRevenuesByPeriodByproductCategory;
                }

                Dictionary<DateTime, PartyProductCategoryRevenue> partyProductCategoryRevenuesByPeriod;
                if (!partyProductCategoryRevenuesByPeriodByproductCategory.TryGetValue(partyProductCategoryRevenue.ProductCategory, out partyProductCategoryRevenuesByPeriod))
                {
                    partyProductCategoryRevenuesByPeriod = new Dictionary<DateTime, PartyProductCategoryRevenue>();
                    partyProductCategoryRevenuesByPeriodByproductCategory[partyProductCategoryRevenue.ProductCategory] = partyProductCategoryRevenuesByPeriod;
                }

                PartyProductCategoryRevenue revenue;
                if (!partyProductCategoryRevenuesByPeriod.TryGetValue(date, out revenue))
                {
                    partyProductCategoryRevenuesByPeriod.Add(date, partyProductCategoryRevenue);
                }
            }

            var revenues = new HashSet<long>();

            var salesInvoices = session.Extent<SalesInvoice>();
            var year = 0;
            foreach (SalesInvoice salesInvoice in salesInvoices)
            {
                if (year != salesInvoice.InvoiceDate.Year)
                {
                    session.Commit();
                    year = salesInvoice.InvoiceDate.Year;
                }

                var date = DateTimeFactory.CreateDate(salesInvoice.InvoiceDate.Year, salesInvoice.InvoiceDate.Month, 01);

                foreach (SalesInvoiceItem salesInvoiceItem in salesInvoice.SalesInvoiceItems)
                {
                    if (salesInvoiceItem.ExistProduct && salesInvoiceItem.Product.ExistPrimaryProductCategory)
                    {
                        foreach (ProductCategory productCategory in salesInvoiceItem.Product.ProductCategories)
                        {
                            CreateProductCategoryRevenue(session, partyProductCategoryRevenuesByPeriodByProductCategoryByParty, date, revenues, salesInvoiceItem, productCategory);
                        }
                    }
                }
            }

            foreach (PartyProductCategoryRevenue partyProductCategoryRevenue in partyProductCategoryRevenues)
            {
                if (!revenues.Contains(partyProductCategoryRevenue.Id))
                {
                    partyProductCategoryRevenue.Delete();
                }
            }
        }

        public static void AppsFindOrCreateAsDependable(ISession session, PartyProductRevenue dependant)
        {
            foreach (ProductCategory productCategory in dependant.Product.ProductCategories)
            {
                var partyProductCategoryRevenues = dependant.Party.PartyProductCategoryRevenuesWhereParty;
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.Year, dependant.Year);
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.Month, dependant.Month);
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.ProductCategory, productCategory);
                var partyProductCategoryRevenue = partyProductCategoryRevenues.First
                                                  ?? new PartyProductCategoryRevenueBuilder(session)
                                                            .WithParty(dependant.Party)
                                                            .WithProductCategory(productCategory)
                                                            .WithYear(dependant.Year)
                                                            .WithMonth(dependant.Month)
                                                            .WithCurrency(dependant.Currency)
                                                            .Build();

                ProductCategoryRevenues.AppsFindOrCreateAsDependable(session, partyProductCategoryRevenue);
                PartyPackageRevenues.AppsFindOrCreateAsDependable(session, partyProductCategoryRevenue);
            }
        }

        public void AppsFindOrCreateAsDependable(ISession session, PartyProductCategoryRevenue dependant)
        {
            foreach (ProductCategory parentCategory in dependant.ProductCategory.Parents)
            {
                var partyProductCategoryRevenues = dependant.Party.PartyProductCategoryRevenuesWhereParty;
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.Year, dependant.Year);
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.Month, dependant.Month);
                partyProductCategoryRevenues.Filter.AddEquals(M.PartyProductCategoryRevenue.ProductCategory, parentCategory);
                var partyProductCategoryRevenue = partyProductCategoryRevenues.First
                                                  ?? new PartyProductCategoryRevenueBuilder(session)
                                                            .WithParty(dependant.Party)
                                                            .WithProductCategory(parentCategory)
                                                            .WithYear(dependant.Year)
                                                            .WithMonth(dependant.Month)
                                                            .WithCurrency(dependant.Currency)
                                                            .Build();

                ProductCategoryRevenues.AppsFindOrCreateAsDependable(session, partyProductCategoryRevenue);

                this.AppsFindOrCreateAsDependable(session, partyProductCategoryRevenue);
            }
        }

        private static void CreateProductCategoryRevenue(
            ISession session,
            Dictionary<Party, Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>>> partyProductCategoryRevenuesByPeriodByProductCategoryByParty,
            DateTime date,
            HashSet<long> revenues,
            SalesInvoiceItem salesInvoiceItem,
            ProductCategory productCategory)
        {
            PartyProductCategoryRevenue partyProductCategoryRevenue;
            Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>> partyProductCategoryRevenuesByPeriodByProductCategory;
            if (!partyProductCategoryRevenuesByPeriodByProductCategoryByParty.TryGetValue(salesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem.BillToCustomer, out partyProductCategoryRevenuesByPeriodByProductCategory))
            {
                partyProductCategoryRevenue = CreatePartyProductCategoryRevenue(session, salesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem, productCategory);

                partyProductCategoryRevenuesByPeriodByProductCategory = new Dictionary<ProductCategory, Dictionary<DateTime, PartyProductCategoryRevenue>>
                        {
                            {
                                productCategory,
                                new Dictionary<DateTime, PartyProductCategoryRevenue> { { date, partyProductCategoryRevenue } }
                            }
                        };

                partyProductCategoryRevenuesByPeriodByProductCategoryByParty[salesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem.BillToCustomer] = partyProductCategoryRevenuesByPeriodByProductCategory;
            }

            Dictionary<DateTime, PartyProductCategoryRevenue> partyProductCategoryRevenuesByPeriod;
            if (!partyProductCategoryRevenuesByPeriodByProductCategory.TryGetValue(productCategory, out partyProductCategoryRevenuesByPeriod))
            {
                partyProductCategoryRevenue = CreatePartyProductCategoryRevenue(session, salesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem, productCategory);

                partyProductCategoryRevenuesByPeriod = new Dictionary<DateTime, PartyProductCategoryRevenue> { { date, partyProductCategoryRevenue } };

                partyProductCategoryRevenuesByPeriodByProductCategory[productCategory] = partyProductCategoryRevenuesByPeriod;
            }

            if (!partyProductCategoryRevenuesByPeriod.TryGetValue(date, out partyProductCategoryRevenue))
            {
                partyProductCategoryRevenue = CreatePartyProductCategoryRevenue(session, salesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem, productCategory);
                partyProductCategoryRevenuesByPeriod.Add(date, partyProductCategoryRevenue);
            }

            revenues.Add(partyProductCategoryRevenue.Id);
            partyProductCategoryRevenue.Revenue += salesInvoiceItem.TotalExVat;
            partyProductCategoryRevenue.Quantity += salesInvoiceItem.Quantity;

            foreach (ProductCategory parent in productCategory.Parents)
            {
                CreateProductCategoryRevenue(session, partyProductCategoryRevenuesByPeriodByProductCategoryByParty, date, revenues, salesInvoiceItem, parent);
            }
        }

        private static PartyProductCategoryRevenue CreatePartyProductCategoryRevenue(ISession session, SalesInvoice invoice, ProductCategory category)
        {
            return new PartyProductCategoryRevenueBuilder(session)
                        .WithParty(invoice.BillToCustomer)
                        .WithYear(invoice.InvoiceDate.Year)
                        .WithMonth(invoice.InvoiceDate.Month)
                        .WithCurrency(Singleton.Instance(session).PreferredCurrency)
                        .WithProductCategory(category)
                        .Build();
        }
    }
}
