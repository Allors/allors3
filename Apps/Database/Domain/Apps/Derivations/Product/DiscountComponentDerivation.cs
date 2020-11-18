// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Database.Domain.Derivations;
    using Allors.Database.Meta;
    using Database.Derivations;

    public class DiscountComponentDerivation : DomainDerivation
    {
        public DiscountComponentDerivation(M m) : base(m, new Guid("C395DB2E-C4A6-4974-BE35-EF2CC70D347D")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(this.M.DiscountComponent.PricedBy),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<DiscountComponent>())
            {
                var internalOrganisations = new Organisations(@this.Strategy.Session).Extent().Where(v => Equals(v.IsInternalOrganisation, true)).ToArray();

                if (!@this.ExistPricedBy && internalOrganisations.Count() == 1)
                {
                    @this.PricedBy = internalOrganisations.First();
                }

                validation.AssertAtLeastOne(@this, this.M.DiscountComponent.Price, this.M.DiscountComponent.Percentage);
                validation.AssertExistsAtMostOne(@this, this.M.DiscountComponent.Price, this.M.DiscountComponent.Percentage);

                if (@this.ExistPrice)
                {
                    if (!@this.ExistCurrency && @this.ExistPricedBy)
                    {
                        @this.Currency = @this.PricedBy.PreferredCurrency;
                    }

                    validation.AssertExists(@this, this.M.BasePrice.Currency);
                }

                if (@this.ExistProduct)
                {
                    @this.Product.AppsOnDeriveVirtualProductPriceComponent();
                }
            }

        }
    }
}
