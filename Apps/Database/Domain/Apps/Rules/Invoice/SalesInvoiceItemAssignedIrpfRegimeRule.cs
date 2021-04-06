// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Derivations;
    using Meta;
    using Database.Derivations;
    using Resources;

    public class SalesInvoiceItemAssignedIrpfRegimeRule : Rule
    {
        public SalesInvoiceItemAssignedIrpfRegimeRule(MetaPopulation m) : base(m, new Guid("559c4d50-b819-420c-9a3c-6289e2daeba6")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.SalesInvoiceItem, m.SalesInvoiceItem.AssignedIrpfRegime),
                new RolePattern(m.SalesInvoice, m.SalesInvoice.InvoiceDate) { Steps =  new IPropertyType[] {this.M.SalesInvoice.SalesInvoiceItems} },
                new RolePattern(m.SalesInvoice, m.SalesInvoice.DerivedIrpfRegime) { Steps =  new IPropertyType[] {this.M.SalesInvoice.SalesInvoiceItems} },
                new AssociationPattern(m.TimeEntryBilling.InvoiceItem) { Steps =  new IPropertyType[] { m.SalesInvoiceItem.SalesInvoiceWhereSalesInvoiceItem }, OfType = m.SalesInvoice },
                new AssociationPattern(m.SalesInvoice.SalesInvoiceItems),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var transaction = cycle.Transaction;
            var validation = cycle.Validation;
            var changeSet = cycle.ChangeSet;

            foreach (var @this in matches.Cast<SalesInvoiceItem>())
            {
                var salesInvoice = @this.SalesInvoiceWhereSalesInvoiceItem;
                
                @this.DerivedIrpfRegime = @this.ExistAssignedIrpfRegime ? @this.AssignedIrpfRegime : @this.SalesInvoiceWhereSalesInvoiceItem?.DerivedIrpfRegime;
                @this.IrpfRate = @this.DerivedIrpfRegime?.IrpfRates.First(v => v.FromDate <= salesInvoice.InvoiceDate && (!v.ExistThroughDate || v.ThroughDate >= salesInvoice.InvoiceDate));
            }
        }
    }
}
