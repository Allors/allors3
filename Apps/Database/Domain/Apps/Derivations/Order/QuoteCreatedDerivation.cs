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

    public class QuoteCreatedDerivation : DomainDerivation
    {
        public QuoteCreatedDerivation(M m) : base(m, new Guid("b66c0721-4aa5-4ca7-91a0-534f6cfc6718")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.Quote.QuoteState),
                new RolePattern(m.Quote.Issuer),
                new RolePattern(m.Quote.Receiver),
                new RolePattern(m.Quote.Locale),
                new RolePattern(m.Quote.AssignedVatRegime),
                new RolePattern(m.Quote.AssignedIrpfRegime),
                new RolePattern(m.Quote.AssignedCurrency),
                new RolePattern(m.Quote.IssueDate),
                new RolePattern(m.Organisation.Locale) { Steps = new IPropertyType[] { m.Organisation.QuotesWhereIssuer }},
                new RolePattern(m.Party.Locale) { Steps = new IPropertyType[] { m.Party.QuotesWhereReceiver }},
                new RolePattern(m.Party.PreferredCurrency) { Steps = new IPropertyType[] { m.Party.QuotesWhereReceiver }},
                new RolePattern(m.Organisation.PreferredCurrency) { Steps = new IPropertyType[] { m.Organisation.QuotesWhereReceiver }},
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<Quote>().Where(v => v.QuoteState.IsCreated))
            {
                @this.DerivedLocale = @this.Locale ?? @this.Receiver?.Locale ?? @this.Issuer?.Locale;
                @this.DerivedCurrency = @this.AssignedCurrency ?? @this.Receiver?.PreferredCurrency ?? @this.Receiver?.Locale?.Country?.Currency ?? @this.Issuer?.PreferredCurrency;
                @this.DerivedVatRegime = @this.AssignedVatRegime;
                @this.DerivedIrpfRegime = @this.AssignedIrpfRegime;

                if (@this.ExistIssueDate)
                {
                    @this.DerivedVatRate = @this.DerivedVatRegime?.VatRates.First(v => v.FromDate <= @this.IssueDate && (!v.ExistThroughDate || v.ThroughDate >= @this.IssueDate));
                    @this.DerivedIrpfRate = @this.DerivedIrpfRegime?.IrpfRates.First(v => v.FromDate <= @this.IssueDate && (!v.ExistThroughDate || v.ThroughDate >= @this.IssueDate));
                }
            }
        }
    }
}
