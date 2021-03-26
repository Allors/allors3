// <copyright file="PurchaseOrderAwaitingApprovalLevel2Derivation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Derivations;
    using Meta;
    using Database.Derivations;
    using Resources;

    public class PurchaseOrderAwaitingApprovalLevel2Derivation : DomainDerivation
    {
        public PurchaseOrderAwaitingApprovalLevel2Derivation(M m) : base(m, new Guid("f865d990-4777-4a52-bfc2-eba6eef183a7")) =>
            this.Patterns = new Pattern[]
            {
                new RolePattern(m.PurchaseOrder, m.PurchaseOrder.PurchaseOrderState)
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<PurchaseOrder>().Where(v => v.PurchaseOrderState.IsAwaitingApprovalLevel2))
            {
                var openTasks = @this.TasksWhereWorkItem.Where(v => !v.ExistDateClosed).ToArray();

                if (!openTasks.OfType<PurchaseOrderApprovalLevel2>().Any())
                {
                    new PurchaseOrderApprovalLevel2Builder(@this.Transaction()).WithPurchaseOrder(@this).Build();
                }
            }
        }
    }
}
