// <copyright file="DomainDerivationCycle.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Validating
{
    using System;
    using System.Collections.Generic;
    using Database.Derivations;
    using Meta;
    using Tracing;

    public class Validator : IDisposable
    {
        public Validator(IOnAccess onAccess, Pattern[] patterns, IEnumerable<IObject> matches)
        {
            this.OnAccess = onAccess;

            this.Matcher = new Matcher();

            foreach (var match in matches)
            {
                foreach (var pattern in patterns)
                {
                    switch (pattern)
                    {
                        case ChangedPattern changedRolePattern:
                            foreach (var changedPropertyMatch in changedRolePattern.CreatePropertyMatches(match.Strategy, this.Matcher))
                            {
                                this.Matcher.Add(changedPropertyMatch, changedRolePattern.RoleType);
                            }

                            break;
                    }
                }
            }

            this.OnAccess.OnAccessUnitRole = this.Validate;
            this.OnAccess.OnAccessCompositeRole = this.Validate;
            this.OnAccess.OnAccessCompositesRole = this.Validate;
            this.OnAccess.OnAccessCompositeAssociation = this.Validate;
            this.OnAccess.OnAccessCompositesAssociation = this.Validate;
        }

        public IOnAccess OnAccess { get; }

        public Matcher Matcher { get; }

        public void Dispose()
        {
            this.OnAccess.OnAccessUnitRole = null;
            this.OnAccess.OnAccessCompositeRole = null;
            this.OnAccess.OnAccessCompositesRole = null;
            this.OnAccess.OnAccessCompositeAssociation = null;
            this.OnAccess.OnAccessCompositesAssociation = null;
        }

        private void Validate(IStrategy strategy, IPropertyType propertyType) => this.Matcher.Match(strategy, propertyType);
    }
}
