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

    public class ShipmentRule : Rule
    {
        public ShipmentRule(M m) : base(m, new Guid("C08727A3-808A-4CB1-B926-DA7432BAAC44")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.Shipment, m.Shipment.DerivationTrigger),
                new RolePattern(m.Shipment, m.Shipment.ShipmentItems),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<Shipment>())
            {
                foreach (ShipmentItem shipmentItem in @this.ShipmentItems)
                {
                    shipmentItem.Sync(@this);
                }
            }
        }
    }
}