
// <copyright file="WorkTaskCanInvoiceDerivation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;
    using Database.Derivations;

    public class WorkTaskCanInvoiceDerivation : DomainDerivation
    {
        public WorkTaskCanInvoiceDerivation(M m) : base(m, new Guid("17ee3e8a-2430-48db-b712-ba305d488459")) =>
            this.Patterns = new Pattern[]
        {
            new ChangedPattern(m.WorkTask.WorkEffortState),
            new ChangedPattern(m.TimeSheet.TimeEntries) { Steps = new IPropertyType[] { m.TimeSheet.TimeEntries, m.TimeEntry.WorkEffort} },
        };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var session = cycle.Session;
            var validation = cycle.Validation;

            foreach (var @this in matches.Cast<WorkTask>())
            {
                // when proforma invoice is deleted then WorkEffortBillingsWhereWorkEffort do not exist and WorkEffortState is Finished
                if (@this.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Completed)
                    || @this.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Finished))
                {
                    @this.CanInvoice = true;

                    if (@this.ExecutedBy.Equals(@this.Customer))
                    {
                        @this.CanInvoice = false;
                    }

                    if (@this.ExistWorkEffortBillingsWhereWorkEffort)
                    {
                        @this.CanInvoice = false;
                    }

                    if (@this.CanInvoice)
                    {
                        foreach (TimeEntry timeEntry in @this.ServiceEntriesWhereWorkEffort)
                        {
                            if (!timeEntry.ExistThroughDate)
                            {
                                @this.CanInvoice = false;
                                break;
                            }

                            if (timeEntry.ExistTimeEntryBillingsWhereTimeEntry)
                            {
                                @this.CanInvoice = false;
                            }
                        }
                    }

                    if (@this.ExistWorkEffortWhereChild)
                    {
                        @this.CanInvoice = false;
                    }

                    if (@this.CanInvoice)
                    {
                        foreach (WorkEffort child in @this.Children)
                        {
                            if (!(child.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Completed)
                                || child.WorkEffortState.Equals(new WorkEffortStates(@this.Strategy.Session).Finished)))
                            {
                                @this.CanInvoice = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    @this.CanInvoice = false;
                }
            }
        }
    }
}