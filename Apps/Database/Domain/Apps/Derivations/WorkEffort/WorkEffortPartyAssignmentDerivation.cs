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
    using Resources;

    public class WorkEffortPartyAssignmentDerivation : DomainDerivation
    {
        public WorkEffortPartyAssignmentDerivation(M m) : base(m, new Guid("072c0e19-ef0c-4ff5-8204-e5071f5ab7f1")) =>
            this.Patterns = new Pattern[]
        {
            new ChangedPattern(m.WorkEffortPartyAssignment.AssignmentRates),
        };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var session = cycle.Session;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<WorkEffortPartyAssignment>())
            {
                if (@this.ExistAssignmentRates)
                {
                    validation.AddError($"{@this}, {@this.Meta.AssignmentRates}, {ErrorMessages.WorkEffortRateError}");
                }
            }
        }
    }
}