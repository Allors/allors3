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
    using Derivations.Rules;

    public class SerialisedItemPartWhereSerialisedItemRule : Rule
    {
        public SerialisedItemPartWhereSerialisedItemRule(MetaPopulation m) : base(m, new Guid("02b0e0bf-7fa6-453d-bef2-8b267979b1ff")) =>
            this.Patterns = new Pattern[]
            {
                m.SerialisedItem.RolePattern(v => v.Name),
                m.SerialisedItem.AssociationPattern(v => v.PartWhereSerialisedItem),
                m.Part.AssociationPattern(v => v.SupplierOfferingsWherePart, v => v.SerialisedItems),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<SerialisedItem>())
            {
                if (!@this.ExistName && @this.ExistPartWhereSerialisedItem)
                {
                    @this.Name = @this.PartWhereSerialisedItem.Name;
                }
            }
        }
    }
}
