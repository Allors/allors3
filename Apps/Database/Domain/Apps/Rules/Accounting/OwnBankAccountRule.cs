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
    using Derivations;

    public class OwnBankAccountRule : Rule
    {
        public OwnBankAccountRule(MetaPopulation m) : base(m, new Guid("0e20e10e-fadf-4bf2-97be-98e0e7b09d0d")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.OwnBankAccount, m.OwnBankAccount.GeneralLedgerAccount),
                new RolePattern(m.OwnBankAccount, m.OwnBankAccount.Journal),
                new AssociationPattern(m.InternalOrganisation.DerivedActiveCollectionMethods) { OfType = m.OwnBankAccount },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<OwnBankAccount>())
            {
                if (@this.ExistInternalOrganisationWhereDerivedActiveCollectionMethod && @this.InternalOrganisationWhereDerivedActiveCollectionMethod.DoAccounting)
                {
                    validation.AssertAtLeastOne(@this, @this.M.Cash.GeneralLedgerAccount, @this.M.Cash.Journal);
                }

                validation.AssertExistsAtMostOne(@this, @this.M.Cash.GeneralLedgerAccount, @this.M.Cash.Journal);
            }
        }
    }
}
