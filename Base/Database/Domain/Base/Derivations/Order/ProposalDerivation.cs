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

    public class ProposalDerivation : IDomainDerivation
    {
        public Guid Id => new Guid("F51A25BD-3FB7-4539-A541-5F19F124AA9F");

        public IEnumerable<Pattern> Patterns { get; } = new[] { new CreatedPattern(M.Proposal.Class) };

        public void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var proposal in matches.Cast<Proposal>())
            {
                foreach (QuoteItem quoteItem in proposal.QuoteItems)
                {
                    quoteItem.Sync(proposal);
                }

                var deletePermission = new Permissions(proposal.Strategy.Session).Get(proposal.Meta.ObjectType, proposal.Meta.Delete, Operations.Execute);
                if (proposal.IsDeletable)
                {
                    proposal.RemoveDeniedPermission(deletePermission);
                }
                else
                {
                    proposal.AddDeniedPermission(deletePermission);
                }
            }
        }
    }
}