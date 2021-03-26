// <copyright file="PartSuppliedByDerivation.cs" company="Allors bvba">
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

    public class PartSuppliedByDerivation : DomainDerivation
    {
        public PartSuppliedByDerivation(M m) : base(m, new Guid("9bdcfcf1-3140-4e89-bea7-41b1662148b1")) =>
            this.Patterns = new Pattern[]
            {
                new AssociationPattern(m.SupplierOffering.Part),
                new RolePattern(m.SupplierOffering.FromDate) { Steps = new IPropertyType[]{ m.SupplierOffering.Part }},
                new RolePattern(m.SupplierOffering.ThroughDate) { Steps = new IPropertyType[]{ m.SupplierOffering.Part } },
                new RolePattern(m.SupplierOffering.AllVersions) { Steps = new IPropertyType[]{ m.SupplierOffering.AllVersions, m.SupplierOfferingVersion.Part } },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<Part>())
            {
                @this.RemoveSuppliedBy();
                foreach (SupplierOffering supplierOffering in @this.SupplierOfferingsWherePart)
                {
                    if (supplierOffering.FromDate <= @this.Transaction().Now()
                        && (!supplierOffering.ExistThroughDate || supplierOffering.ThroughDate >= @this.Transaction().Now()))
                    {
                        @this.AddSuppliedBy(supplierOffering.Supplier);
                    }
                }
            }
        }
    }
}
