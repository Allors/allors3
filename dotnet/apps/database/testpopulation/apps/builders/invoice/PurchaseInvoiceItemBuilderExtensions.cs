// <copyright file="PurchaseInvoiceItemBuilderExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary></summary>

namespace Allors.Database.Domain.TestPopulation
{
    using System.Linq;

    public static partial class PurchaseInvoiceItemBuilderExtensions
    {
        public static PurchaseInvoiceItemBuilder WithDefaults(this PurchaseInvoiceItemBuilder @this)
        {
            var faker = @this.Transaction.Faker();
            var invoiceItemTypes = @this.Transaction.Extent<InvoiceItemType>().ToList();

            var otherInvoiceItemTypes = invoiceItemTypes.Except(
                invoiceItemTypes.Where(v => v.UniqueId.Equals(InvoiceItemTypes.ProductItemId) || v.UniqueId.Equals(InvoiceItemTypes.PartItemId)).ToList())
                .ToList();

            @this.WithDescription(faker.Lorem.Sentences(2));
            @this.WithComment(faker.Lorem.Sentence());
            @this.WithInternalComment(faker.Lorem.Sentence());
            @this.WithInvoiceItemType(faker.Random.ListItem(otherInvoiceItemTypes));
            @this.WithQuantity(1);
            @this.WithAssignedUnitPrice(faker.Random.UInt());

            return @this;
        }

        public static PurchaseInvoiceItemBuilder WithSerialisedProductItemDefaults(this PurchaseInvoiceItemBuilder @this)
        {
            var m = @this.Transaction.Database.Services().M;
            var faker = @this.Transaction.Faker();
            var invoiceItemType = @this.Transaction.Extent<InvoiceItemType>().FirstOrDefault(v => v.UniqueId.Equals(InvoiceItemTypes.ProductItemId));

            var unifiedGoodExtent = @this.Transaction.Extent<UnifiedGood>();
            unifiedGoodExtent.Filter.AddEquals(m.UnifiedGood.InventoryItemKind, new InventoryItemKinds(@this.Transaction).Serialised);
            var serializedProduct = unifiedGoodExtent.First();

            @this.WithDescription(faker.Lorem.Sentences(2));
            @this.WithComment(faker.Lorem.Sentence());
            @this.WithInternalComment(faker.Lorem.Sentence());
            @this.WithInvoiceItemType(invoiceItemType);
            @this.WithPart(serializedProduct);
            @this.WithSerialisedItem(serializedProduct.SerialisedItems.First);
            @this.WithQuantity(1);
            @this.WithAssignedUnitPrice(faker.Random.UInt());

            return @this;
        }

        public static PurchaseInvoiceItemBuilder WithPartItemDefaults(this PurchaseInvoiceItemBuilder @this)
        {
            var m = @this.Transaction.Database.Services().M;
            var faker = @this.Transaction.Faker();
            var invoiceItemType = @this.Transaction.Extent<InvoiceItemType>().FirstOrDefault(v => v.UniqueId.Equals(InvoiceItemTypes.PartItemId));

            var unifiedGoodExtent = @this.Transaction.Extent<UnifiedGood>();
            unifiedGoodExtent.Filter.AddEquals(m.UnifiedGood.InventoryItemKind, new InventoryItemKinds(@this.Transaction).Serialised);
            var serializedPart = unifiedGoodExtent.First();

            @this.WithDescription(faker.Lorem.Sentences(2));
            @this.WithComment(faker.Lorem.Sentence());
            @this.WithInternalComment(faker.Lorem.Sentence());
            @this.WithInvoiceItemType(invoiceItemType);
            @this.WithPart(serializedPart);
            @this.WithSerialisedItem(serializedPart.SerialisedItems.First);
            @this.WithQuantity(1);
            @this.WithAssignedUnitPrice(faker.Random.UInt());

            return @this;
        }
    }
}