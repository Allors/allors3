// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Model.cs" company="Allors bvba">
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

namespace Allors.Domain.Print.SalesOrderModel
{
    using System.Linq;

    public class Model
    {
        public Model(SalesOrder order)
        {
            var session = order.Strategy.Session;

            this.Order = new OrderModel(order);
            
            this.TakenBy = new TakenByModel((Organisation)order.TakenBy);
            this.BillTo = new BillToModel(order);
            this.ShipTo = new ShipToModel(order);

            this.OrderItems = order.SalesOrderItems.Select(v => new OrderItemModel(v)).ToArray();

            var paymentTerm = new InvoiceTermTypes(session).PaymentNetDays;
            this.SalesTerms = order.SalesTerms.Where(v => !v.TermType.Equals(paymentTerm)).Select(v => new SalesTermModel(v)).ToArray();

            string TakenByCountry = null;
            if (order.TakenBy.PartyContactMechanisms?.FirstOrDefault(v => v.ContactPurposes.Any(p => Equals(p, new ContactMechanismPurposes(session).RegisteredOffice)))?.ContactMechanism is PostalAddress registeredOffice)
            {
                TakenByCountry = registeredOffice.Country.IsoCode;
            }

            if (TakenByCountry == "BE")
            {
                this.VatClause = order.DerivedVatClause?.LocalisedClause.First(v => v.Locale.Equals(new Locales(session).DutchNetherlands)).Text;

                if (Equals(order.DerivedVatClause, new VatClauses(session).BeArt14Par2))
                {
                    var shipToCountry = order.ShipToAddress?.Country?.Name;
                    this.VatClause = this.VatClause.Replace("{shipToCountry}", shipToCountry);
                }
            }
        }

        public OrderModel Order { get; }

        public TakenByModel TakenBy { get; }

        public BillToModel BillTo { get; }

        public ShipToModel ShipTo { get; }

        public OrderItemModel[] OrderItems { get; }

        public SalesTermModel[] SalesTerms { get; }

        public string VatClause { get; }
    }
}
