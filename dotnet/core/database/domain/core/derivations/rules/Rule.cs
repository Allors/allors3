
// <copyright file="ValidationBase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Rules
{
    using System;
    using System.Collections.Generic;
    using Meta;

    public abstract partial class Rule : IRule
    {
        protected Rule(MetaPopulation m, Guid id)
        {
            this.M = m;
            this.Id = id;
        }

        public MetaPopulation M { get; }

        public Guid Id { get; }

        IPattern[] IRule.Patterns => this.Patterns;
        public Pattern[] Patterns { get; protected set; }

        public abstract void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches);
    }
}
