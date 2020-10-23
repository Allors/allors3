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

    public class AgreementTermDerivation : DomainDerivation
    {
        public AgreementTermDerivation(M m) : base(m, new Guid("2F28CF03-571A-4F7B-B71C-D8ACEBC734AC")) =>
            this.Patterns = new Pattern[]
            {
                new CreatedPattern(this.M.AgreementTerm.Interface),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var agreementTerm in matches.Cast<AgreementTerm>())
            {
                cycle.Validation.AssertAtLeastOne(agreementTerm, this.M.AgreementTerm.TermType, this.M.AgreementTerm.Description);
            }
        }
    }
}