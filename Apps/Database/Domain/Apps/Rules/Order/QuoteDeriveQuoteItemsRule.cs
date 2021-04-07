// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Database.Derivations;
    using Meta;

    public class QuoteDeriveQuoteItemsRule : Rule
    {
        public QuoteDeriveQuoteItemsRule(MetaPopulation m) : base(m, new Guid("00728229-8ff0-4f2b-b34b-f62010706a95")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.ProductQuote, m.ProductQuote.QuoteItems),
                new RolePattern(m.QuoteItem, m.QuoteItem.QuoteItemState) { Steps =  new IPropertyType[] {m.QuoteItem.QuoteWhereQuoteItem} },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<Quote>())
            {
                @this.ValidQuoteItems = @this.QuoteItems.Where(v => v.IsValid).ToArray();

                foreach (QuoteItem quoteItem in @this.QuoteItems)
                {
                    quoteItem.Sync(@this);
                }
            }
        }
    }
}