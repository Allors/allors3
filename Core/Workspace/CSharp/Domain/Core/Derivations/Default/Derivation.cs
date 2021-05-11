// <copyright file="Cycle.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Derivations.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Derivation : IDerivation
    {
        public Derivation(ISession session, Engine engine, int maxDomainDerivationCycles)
        {
            this.Session = session;
            this.Engine = engine;
            this.MaxCycles = maxDomainDerivationCycles;

            this.Validation = new Validation();
        }

        public ISession Session { get; }

        public Engine Engine { get; }

        public int MaxCycles { get; }

        public IValidation Validation { get; }

        public IValidation Execute()
        {
            var cycles = 0;

            var changeSet = this.Session.Checkpoint();

            while (changeSet.RolesByAssociationType?.Count > 0 || changeSet.AssociationsByRoleType?.Count > 0 || changeSet.Created?.Count > 0 || changeSet.Instantiated?.Count > 0)
            {
                if (++cycles > this.MaxCycles)
                {
                    throw new Exception("Maximum amount of domain derivation cycles detected");
                }

                var cycle = new Cycle
                {
                    ChangeSet = changeSet,
                    Session = this.Session,
                    Validation = this.Validation
                };

                var matchesByRule = new Dictionary<IRule, ISet<IObject>>();

                if (changeSet.Instantiated != null)
                {
                    foreach (var instantiated in changeSet?.Instantiated)
                    {
                        var @class = instantiated.Class;
                        if (this.Engine.RulesByClass.TryGetValue(@class, out var rules))
                        {
                            foreach (var rule in rules)
                            {
                                if (!matchesByRule.TryGetValue(rule, out var matches))
                                {
                                    matches = new HashSet<IObject>();
                                    matchesByRule.Add(rule, matches);
                                }

                                _ = matches.Add(instantiated.Object);
                            }
                        }
                    }
                }

                foreach (var kvp in changeSet.AssociationsByRoleType)
                {
                    var roleType = kvp.Key;
                    var associations = kvp.Value;

                    foreach (var association in associations)
                    {
                        var @class = association.Class;

                        if (this.Engine.PatternsByRoleTypeByClass.TryGetValue(@class, out var patternsByRoleType))
                        {
                            if (patternsByRoleType.TryGetValue(roleType, out var patterns))
                            {
                                foreach (var pattern in patterns)
                                {
                                    var rule = this.Engine.RuleByPattern[pattern];
                                    if (!matchesByRule.TryGetValue(rule, out var matches))
                                    {
                                        matches = new HashSet<IObject>();
                                        matchesByRule.Add(rule, matches);
                                    }

                                    IEnumerable<IObject> source = new IObject[] { association.Object };

                                    if (pattern.Tree != null)
                                    {
                                        source = source.SelectMany(v => pattern.Tree.SelectMany(w => w.Resolve(v)));
                                    }

                                    if (pattern.OfType != null)
                                    {
                                        source = source.Where(v => pattern.OfType.IsAssignableFrom(v.Strategy.Class));
                                    }

                                    matches.UnionWith(source);
                                }
                            }
                        }
                    }
                }

                foreach (var kvp in changeSet.RolesByAssociationType)
                {
                    var associationType = kvp.Key;
                    var roles = kvp.Value;

                    foreach (var role in roles)
                    {
                        var @class = role.Class;

                        if (this.Engine.PatternsByAssociationTypeByClass.TryGetValue(@class, out var patternsByAssociationType))
                        {
                            if (patternsByAssociationType.TryGetValue(associationType, out var patterns))
                            {
                                foreach (var pattern in patterns)
                                {
                                    var rule = this.Engine.RuleByPattern[pattern];
                                    if (!matchesByRule.TryGetValue(rule, out var matches))
                                    {
                                        matches = new HashSet<IObject>();
                                        matchesByRule.Add(rule, matches);
                                    }

                                    IEnumerable<IObject> source = new IObject[] { role.Object };

                                    if (pattern.Tree != null)
                                    {
                                        source = source.SelectMany(v => pattern.Tree.SelectMany(w => w.Resolve(v)));
                                    }

                                    if (pattern.OfType != null)
                                    {
                                        source = source.Where(v => pattern.OfType.IsAssignableFrom(v.Strategy.Class));
                                    }

                                    matches.UnionWith(source);
                                }
                            }
                        }
                    }
                }

                foreach (var kvp in matchesByRule)
                {
                    var domainDerivation = kvp.Key;
                    var matches = kvp.Value;
                    domainDerivation.Derive(cycle, matches);
                }

                changeSet = this.Session.Checkpoint();
            }

            return this.Validation;
        }
    }
}
