// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;
    using Database.Derivations;
    using Resources;

    public class ProductQuoteDerivation : DomainDerivation
    {
        public ProductQuoteDerivation(M m) : base(m, new Guid("6F421122-37A0-4F8E-A08A-996F16CC0218")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.ProductQuote, m.ProductQuote.Issuer),
                new RolePattern(m.ProductQuote, m.ProductQuote.QuoteNumber),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<ProductQuote>())
            {
                if (@this.ExistCurrentVersion
                    && @this.CurrentVersion.ExistIssuer
                    && @this.Issuer != @this.CurrentVersion.Issuer)
                {
                    validation.AddError($"{@this} {this.M.Proposal.Issuer} {ErrorMessages.InternalOrganisationChanged}");
                }

                @this.WorkItemDescription = $"ProductQuote: {@this.QuoteNumber} [{@this.Issuer?.PartyName}]";

                @this.ResetPrintDocument();
            }
        }
    }
}
