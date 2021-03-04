// <copyright file="DomainDerivationCycle.cs" company="Allors bvba">
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

    public class DomainDerive
    {
        public DomainDerive(ISession session, int maxDomainDerivationCycles)
        {
            this.Session = session;
            this.MaxDomainDerivationCycles = maxDomainDerivationCycles;

            this.Validation = new DomainValidation();
        }

        public ISession Session { get; }

        public int MaxDomainDerivationCycles { get; }

        public IDomainValidation Validation { get; }

        public void Execute()
        {
            // Domain Derivations
            var domainCycles = 0;

            var domainDerivationById = this.Session.Workspace.DomainDerivationById;
            if (domainDerivationById.Any())
            {
                var changeSet = this.Session.Checkpoint();

                while (changeSet.Associations.Any() || changeSet.Roles.Any() || changeSet.Created.Any() || changeSet.Deleted.Any())
                {
                    var session = changeSet.Session;

                    if (++domainCycles > this.MaxDomainDerivationCycles)
                    {
                        throw new Exception("Maximum amount of domain derivation cycles detected");
                    }

                    var domainCycle = new DomainDerivationCycle
                    {
                        ChangeSet = changeSet,
                        Session = session,
                        Validation = this.Validation
                    };

                    var matchesByDerivation = new Dictionary<IDomainDerivation, IEnumerable<IObject>>();
                    foreach (var kvp in domainDerivationById)
                    {
                        var domainDerivation = kvp.Value;
                        var matches = new HashSet<IObject>();

                        foreach (var pattern in domainDerivation.Patterns)
                        {
                            var source = pattern switch
                            {
                                // RoleDefault
                                AssociationPattern { RoleType: RoleDefault roleType } => changeSet
                                    .AssociationsByRoleType
                                    .Where(v => v.Key.RelationType.Equals(roleType.RelationType))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value)),

                                RolePattern { RoleType: RoleDefault roleType } => changeSet
                                    .RolesByAssociationType
                                    .Where(v => v.Key.RelationType.Equals(roleType.RelationType))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value)),

                                // RoleInterface
                                AssociationPattern { RoleType: RoleInterface roleInterface } => changeSet
                                    .AssociationsByRoleType
                                    .Where(v => v.Key.RelationType.Equals(roleInterface.RelationType))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value))
                                    .Where(v =>
                                        roleInterface.AssociationTypeComposite.IsAssignableFrom(v.Strategy.Class)),

                                RolePattern { RoleType: RoleInterface roleInterface } => changeSet
                                    .RolesByAssociationType
                                    .Where(v => v.Key.RelationType.Equals(roleInterface.RelationType))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value)),

                                // RoleClass
                                AssociationPattern { RoleType: RoleClass roleClass } => changeSet
                                    .AssociationsByRoleType.Where(v => v.Key.Equals(roleClass))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value))
                                    .Where(v => v.Strategy.Class.Equals(roleClass.AssociationTypeComposite)),

                                RolePattern { RoleType: RoleClass roleClass } => changeSet
                                    .RolesByAssociationType.Where(v => v.Key.RoleType.Equals(roleClass))
                                    .SelectMany(v => session.Instantiate<IObject>(v.Value)),

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
                            domainDerivation.Derive(domainCycle, matches);
                        }
                    }

                    changeSet = this.Session.Checkpoint();
                }
            }
        }
    }
}
