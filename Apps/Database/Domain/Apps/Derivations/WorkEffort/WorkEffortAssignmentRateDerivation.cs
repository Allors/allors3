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
    using Resources;

    public class WorkEffortAssignmentRateDerivation : DomainDerivation
    {
        public WorkEffortAssignmentRateDerivation(M m) : base(m, new Guid("a733672e-cbd8-4816-b628-0ab9bdd55703")) =>
            this.Patterns = new Pattern[]
        {
            new AssociationPattern(this.M.WorkEffortAssignmentRate.WorkEffort),
            new AssociationPattern(this.M.WorkEffortAssignmentRate.WorkEffortPartyAssignment),
            new AssociationPattern(this.M.WorkEffortAssignmentRate.RateType),
        };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var transaction = cycle.Transaction;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<WorkEffortAssignmentRate>())
            {
                if (!@this.ExistWorkEffort && @this.ExistWorkEffortPartyAssignment)
                {
                    @this.WorkEffort = @this.WorkEffortPartyAssignment.Assignment;
                }

                if (@this.ExistWorkEffort && @this.ExistRateType)
                {
                    var extent = @this.WorkEffort.WorkEffortAssignmentRatesWhereWorkEffort;
                    extent.Filter.AddEquals(@this.M.WorkEffortAssignmentRate.RateType, @this.RateType);
                    if (extent.Count > 1)
                    {
                        validation.AddError($"{@this}, {@this.Meta.RateType}, {ErrorMessages.WorkEffortRateError}");
                    }
                }
            }
        }
    }
}
