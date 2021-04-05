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

    public class RoleOne2OneRule : Rule
    {
        public RoleOne2OneRule(MetaPopulation m) : base(m, new Guid("1C369F4C-CC12-4064-9261-BF899205E251")) =>
            this.Patterns = new[]
            {
                new RolePattern(m.CC, m.CC.Assigned) {Steps = new IPropertyType[]{m.CC.BBWhereOne2One, m.BB.AAWhereOne2One}},
                new RolePattern(m.CC, m.CC.Assigned) {Steps = new IPropertyType[]{m.CC.BBWhereUnusedOne2One, m.BB.AAWhereUnusedOne2One}},
            };


        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var aa in matches.Cast<AA>())
            {
                aa.Derived = aa.One2One?.One2One?.Assigned;
            }
        }
    }
}
