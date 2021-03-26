// <copyright file="QuoteItemCreatedDerivation.cs" company="Allors bvba">
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

    public class QuoteItemCreatedDerivation : DomainDerivation
    {
        public QuoteItemCreatedDerivation(M m) : base(m, new Guid("b66c0721-4aa5-4ca7-91a0-534f6cfc6718")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.QuoteItem.AssignedVatRegime),
                new RolePattern(m.QuoteItem.AssignedIrpfRegime),
                new AssociationPattern(m.Quote.QuoteItems),
                new RolePattern(m.Quote.IssueDate) { Steps = new IPropertyType[] { m.Quote.QuoteItems }},
                new RolePattern(m.Quote.DerivedVatRegime) { Steps = new IPropertyType[] { m.Quote.QuoteItems }},
                new RolePattern(m.Quote.DerivedIrpfRegime) { Steps = new IPropertyType[] { m.Quote.QuoteItems }},
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<QuoteItem>())
            {
                var quote = @this.QuoteWhereQuoteItem;

                if (quote.QuoteState.IsCreated)
                {
                    @this.DerivedVatRegime = @this.AssignedVatRegime ?? quote.DerivedVatRegime;
                    @this.VatRate = @this.DerivedVatRegime?.VatRates.First(v => v.FromDate <= quote.IssueDate && (!v.ExistThroughDate || v.ThroughDate >= quote.IssueDate));

                    @this.DerivedIrpfRegime = @this.AssignedIrpfRegime ?? quote.DerivedIrpfRegime;
                    @this.IrpfRate = @this.DerivedIrpfRegime?.IrpfRates.First(v => v.FromDate <= quote.IssueDate && (!v.ExistThroughDate || v.ThroughDate >= quote.IssueDate));
                }
            }
        }
    }
}
