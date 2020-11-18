// <copyright file="InvoiceItemModel.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Print.SalesInvoiceModel
{
    using System.Globalization;
    using Markdig;

    public class InvoiceItemModel
    {
        public InvoiceItemModel(SalesInvoiceItem item)
        {
            var currencyIsoCode = item.SalesInvoiceWhereSalesInvoiceItem.Currency.IsoCode;

            this.Reference = item.InvoiceItemType?.Name;

            this.Product = item.ExistProduct ? item.Product?.Name : item.Part?.Name;

            var description = item.Description;
            if (description != null)
            {
                description = Markdown.ToPlainText(description);
            }

            this.Description = description?.Split('\n');

            this.Quantity = item.Quantity;
            // TODO: Where does the currency come from?
            this.Price = item.UnitPrice.ToString("N2", new CultureInfo("nl-BE"));
            this.Amount = item.TotalExVat.ToString("N2", new CultureInfo("nl-BE"));
            this.Comment = item.Comment?.Split('\n');
        }

        public string Reference { get; }

        public string Product { get; }

        public string[] Description { get; }

        public decimal Quantity { get; }

        public string Price { get; }

        public string Amount { get; }

        public string[] Comment { get; }
    }
}
