// <copyright file="Or.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using Database.Data;

    public class Or : ICompositePredicate
    {
        public string[] Dependencies { get; set; }

        public Or(params IPredicate[] operands) => this.Operands = operands;

        public IPredicate[] Operands { get; set; }

        bool IPredicate.ShouldTreeShake(IDictionary<string, string> parameters) => this.HasMissingDependencies(parameters) || this.Operands.All(v => v.ShouldTreeShake(parameters));

        bool IPredicate.HasMissingArguments(IDictionary<string, string> parameters) => this.Operands.All(v => v.HasMissingArguments(parameters));

        void IPredicateContainer.AddPredicate(IPredicate predicate) => this.Operands = this.Operands.Append(predicate).ToArray();

        void IPredicate.Build(ISession session, IDictionary<string, string> parameters, Database.ICompositePredicate compositePredicate)
        {
            var or = compositePredicate.AddOr();
            foreach (var predicate in this.Operands)
            {
                if (!predicate.ShouldTreeShake(parameters))
                {
                    predicate.Build(session, parameters, or);
                }
            }
        }

        public void Accept(IVisitor visitor) => visitor.VisitOr(this);
    }
}
