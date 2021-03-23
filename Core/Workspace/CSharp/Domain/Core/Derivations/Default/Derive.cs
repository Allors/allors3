// <copyright file="Cycle.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Derivations.Default
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Data;
    using Meta;
    using Domain;

    public class Derive
    {
        public Derive(ISession session, int maxDomainDerivationCycles)
        {
            this.Session = session;
            this.MaxDomainDerivationCycles = maxDomainDerivationCycles;

            this.Validation = new Validation();
        }

        public ISession Session { get; }

        public int MaxDomainDerivationCycles { get; }

        public IValidation Validation { get; }

        public void Execute()
        {
            var cycles = 0;

            var changeSet = this.Session.Checkpoint();

            while (changeSet.RoleByAssociationType.Count > 0 || changeSet.AssociationByRoleType.Count > 0 || changeSet.Created.Count > 0 || changeSet.Instantiated.Count > 0)
            {
                var session = changeSet.Session;

                if (++cycles > this.MaxDomainDerivationCycles)
                {
                    throw new Exception("Maximum amount of domain derivation cycles detected");
                }

                var cycle = new Cycle
                {
                    ChangeSet = changeSet,
                    Session = session,
                    Validation = this.Validation
                };

                foreach (var domainDerivation in this.Session.Workspace.Derivations)
                {
                    var matches = new HashSet<IObject>();

                    foreach (var pattern in domainDerivation.Patterns)
                    {
                        var source = pattern switch
                        {
                            // RoleDefault
                            AssociationPattern { RoleType: RoleDefault roleType } => changeSet
                                .AssociationByRoleType
                                .Where(v => v.Key.RelationType.Equals(roleType.RelationType))
                                .SelectMany(v => v.Value)
                                .Select(v => v.Object),

                            RolePattern { RoleType: RoleDefault roleType } => changeSet
                                .RoleByAssociationType
                                .Where(v => v.Key.RelationType.Equals(roleType.RelationType))
                                .SelectMany(v => v.Value)
                                .Select(v => v.Object),

                            // RoleInterface
                            AssociationPattern { RoleType: RoleInterface roleInterface } => changeSet
                                .AssociationByRoleType
                                .Where(v => v.Key.RelationType.Equals(roleInterface.RelationType))
                                .SelectMany(v => v.Value)
                                .Where(v => roleInterface.AssociationTypeComposite.IsAssignableFrom(v.Class))
                                .Select(v => v.Object),

                            RolePattern { RoleType: RoleInterface roleInterface } => changeSet
                                .RoleByAssociationType
                                .Where(v => v.Key.RelationType.Equals(roleInterface.RelationType))
                                .SelectMany(v => v.Value)
                                .Select(v => v.Object),

                            // RoleClass
                            AssociationPattern { RoleType: RoleClass roleClass } => changeSet
                                .AssociationByRoleType.Where(v => v.Key.Equals(roleClass))
                                .SelectMany(v => v.Value)
                                .Where(v => v.Class.Equals(roleClass.AssociationTypeComposite))
                                .Select(v => v.Object),

                            RolePattern { RoleType: RoleClass roleClass } => changeSet
                                .RoleByAssociationType.Where(v => v.Key.RoleType.Equals(roleClass))
                                .SelectMany(v => v.Value)
                                .Select(v => v.Object),

                            _ => Array.Empty<IObject>()
                        };

                        if (source != null)
                        {
                            if (pattern.Steps?.Length > 0)
                            {
                                var step = new Step(pattern.Steps);
                                source = source.SelectMany(v => step.Get(v));
                            }

                            if (pattern.OfType != null)
                            {
                                source = source.Where(v => pattern.OfType.IsAssignableFrom(v.Strategy.Class));
                            }

                            matches.UnionWith(source);
                        }
                    }

                    if (matches.Count > 0)
                    {
                        domainDerivation.Derive(cycle, matches);
                    }
                }

                changeSet = this.Session.Checkpoint();
            }
        }
    }
}
