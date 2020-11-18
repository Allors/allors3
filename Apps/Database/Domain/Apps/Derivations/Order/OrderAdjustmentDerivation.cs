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

    public class OrderAdjustmentDerivation : DomainDerivation
    {
        public OrderAdjustmentDerivation(M m) : base(m, new Guid("324777D9-18B4-4601-A64E-66C87947A751")) =>
            this.Patterns = new[]
            {
                new ChangedPattern(this.M.OrderAdjustment.Amount)
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<OrderAdjustment>())
            {
                cycle.Validation.AssertAtLeastOne(@this, this.M.OrderAdjustment.Amount, this.M.ShippingAndHandlingCharge.Percentage);
                cycle.Validation.AssertExistsAtMostOne(@this, this.M.OrderAdjustment.Amount, this.M.ShippingAndHandlingCharge.Percentage);
            }
        }
    }
}
