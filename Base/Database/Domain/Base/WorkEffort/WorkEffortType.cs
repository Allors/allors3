// <copyright file="WorkEffortType.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System.Linq;

    public partial class WorkEffortType
    {
        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            derivation.Validation.AssertExists(this, this.M.WorkEffortType.Description);

            this.CurrentWorkEffortPartStandards = this.WorkEffortPartStandards
                .Where(v => v.FromDate <= this.Session().Now() && (!v.ExistThroughDate || v.ThroughDate >= this.Session().Now()))
                .ToArray();

            this.InactiveWorkEffortPartStandards = this.WorkEffortPartStandards
                .Except(this.CurrentWorkEffortPartStandards)
                .ToArray();
        }
    }
}
