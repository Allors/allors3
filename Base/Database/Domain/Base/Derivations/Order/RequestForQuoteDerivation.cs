// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Meta;

    public class RequestForQuoteDerivation : IDomainDerivation
    {
        public Guid Id => new Guid("BD181210-419E-4F87-8B3C-3AEF43711514");

        public IEnumerable<Pattern> Patterns { get; } = new[] { new CreatedPattern(M.RequestForQuote.Class) };

        public void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var requestForQuote in matches.Cast<RequestForQuote>())
            {
                //session.Prefetch(requestForQuote.SyncPrefetch, requestForQuote);
                foreach (RequestItem requestItem in requestForQuote.RequestItems)
                {
                    requestItem.Sync(requestForQuote);
                }

                if (!requestForQuote.ExistOriginator)
                {
                    requestForQuote.AddDeniedPermission(new Permissions(requestForQuote.Strategy.Session).Get(requestForQuote.Meta.Class, requestForQuote.Meta.Submit, Operations.Execute));
                }

                var deletePermission = new Permissions(requestForQuote.Strategy.Session).Get(requestForQuote.Meta.ObjectType, requestForQuote.Meta.Delete, Operations.Execute);
                if (requestForQuote.IsDeletable())
                {
                    requestForQuote.RemoveDeniedPermission(deletePermission);
                }
                else
                {
                    requestForQuote.AddDeniedPermission(deletePermission);
                }
            }
        }
    }
}
