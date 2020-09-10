// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Domain.Derivations;
    using Allors.Meta;

    public class OrderValueDerivation : IDomainDerivation
    {
        public Guid Id => new Guid("F851B888-11BE-4FD4-A96C-F8760853AA43");

        public IEnumerable<Pattern> Patterns { get; } = new Pattern[]
        {
                new CreatedPattern(M.OrderValue.Class),
        };

        public void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var orderValue in matches.Cast<OrderValue>())
            {
                cycle.Validation.AssertAtLeastOne(orderValue, M.OrderValue.FromAmount, M.OrderValue.ThroughAmount);
            }
        }
    }
}