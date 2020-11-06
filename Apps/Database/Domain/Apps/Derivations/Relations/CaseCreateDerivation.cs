// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Meta;

    public class CaseCreateDerivation : DomainDerivation
    {
        public CaseCreateDerivation(M m) : base(m, new Guid("5dfecf7c-6d26-4edf-bcf2-27ce051d4ced")) =>
            this.Patterns = new Pattern[]
            {
                new CreatedPattern(m.Case.Class),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;
            var session = cycle.Session;

            foreach (var @this in matches.Cast<Case>())
            {
                if (!@this.ExistCaseState)
                {
                    @this.CaseState = new CaseStates(@this.Strategy.Session).Opened;
                }
            }
        }
    }
}
