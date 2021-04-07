// <copyright file="NonUnifiedPartDerivation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Meta;
    using Database.Derivations;

    public class NonUnifiedPartProductIdentificationsRule : Rule
    {
        public NonUnifiedPartProductIdentificationsRule(MetaPopulation m) : base(m, new Guid("6178ee81-e432-48e3-97a3-b25ee4d36c3c")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.NonUnifiedPart, m.NonUnifiedPart.ProductIdentifications),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<NonUnifiedPart>())
            {
                var identifications = @this.ProductIdentifications;
                identifications.Filter.AddEquals(this.M.ProductIdentification.ProductIdentificationType, new ProductIdentificationTypes(@this.Strategy.Transaction).Part);
                var partIdentification = identifications.FirstOrDefault();

                @this.ProductNumber = partIdentification?.Identification;
            }
        }
    }
}