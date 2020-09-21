// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;

    public class AuditableDerivation : IDomainDerivation
    {
        public AuditableDerivation(M m) =>
            this.Patterns = new[]
            {
                new CreatedPattern(m.Auditable.Interface),
                // new ChangedPattern(M.Auditable.Interface)
            };

        public Guid Id => new Guid("a310ed3b-2129-4bee-8457-ae3c8441597f");

        public IEnumerable<Pattern> Patterns { get; }

        public void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var auditable in matches.Cast<Auditable>())
            {
                var user = auditable.Strategy.Session.Scope().User;
                if (user != null)
                {
                    if (cycle.ChangeSet.Created.Contains(auditable.Strategy))
                    {
                        auditable.CreationDate = auditable.Strategy.Session.Now();
                        auditable.CreatedBy = user;
                    }

                    if (cycle.ChangeSet.Associations.Contains(auditable.Id))
                    {
                        auditable.LastModifiedDate = auditable.Strategy.Session.Now();
                        auditable.LastModifiedBy = user;
                    }
                }
            }
        }
    }
}
