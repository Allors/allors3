// <copyright file="AccountingPeriod.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System.Text;

    public partial class AccountingPeriod
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[]{
            new TransitionalConfiguration(this.M.AccountingPeriod, this.M.AccountingPeriod.BudgetState),
        };

        public AccountingPeriod AddNextMonth() => this.AppsAddNextMonth();

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistActive)
            {
                this.Active = true;
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var stringBuilder = new StringBuilder();
            if (this.ExistFromDate)
            {
                stringBuilder.AppendFormat("{0:d}", this.FromDate);
            }

            if (this.ExistThroughDate)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" through ");
                }

                stringBuilder.AppendFormat("{0:d}", this.ThroughDate);
            }
        }

        private AccountingPeriod AppsAddNextMonth()
        {
            var allPeriods = new AccountingPeriods(this.Strategy.Session).Extent();
            allPeriods.Filter.AddEquals(this.Meta.Frequency, new TimeFrequencies(this.Strategy.Session).Month);
            allPeriods.AddSort(this.Meta.FromDate, SortDirection.Descending);

            var mostRecentMonth = allPeriods.First;

            var newMonth = new AccountingPeriodBuilder(this.Strategy.Session)
                .WithPeriodNumber(mostRecentMonth.PeriodNumber + 1)
                .WithFrequency(new TimeFrequencies(this.Strategy.Session).Month)
                .Build();

            if (newMonth.PeriodNumber < 13)
            {
                newMonth.FromDate = mostRecentMonth.FromDate.AddMonths(1).Date;
                newMonth.ThroughDate = mostRecentMonth.FromDate.AddMonths(2).AddSeconds(-1).Date;
            }
            else
            {
                newMonth.FromDate = mostRecentMonth.FromDate;
                newMonth.ThroughDate = mostRecentMonth.ThroughDate;
            }

            if (newMonth.PeriodNumber == 4 || newMonth.PeriodNumber == 7 || newMonth.PeriodNumber == 10)
            {
                newMonth.Parent = this.AppsAddNextQuarter(mostRecentMonth.Parent);
            }
            else
            {
                newMonth.Parent = mostRecentMonth.Parent;
            }

            return newMonth;
        }

        private AccountingPeriod AppsAddNextQuarter(AccountingPeriod previousPeriod)
        {
            var newQuarter = new AccountingPeriodBuilder(this.Strategy.Session)
                .WithPeriodNumber(previousPeriod.PeriodNumber + 1)
                .WithFrequency(new TimeFrequencies(this.Strategy.Session).Trimester)
                .WithFromDate(previousPeriod.FromDate.AddMonths(3).Date)
                .WithThroughDate(previousPeriod.FromDate.AddMonths(6).AddSeconds(-1).Date)
                .Build();

            if (newQuarter.PeriodNumber == 3)
            {
                newQuarter.Parent = this.AppsAddNextSemester(previousPeriod.Parent);
            }
            else
            {
                newQuarter.Parent = previousPeriod.Parent;
            }

            return newQuarter;
        }

        private AccountingPeriod AppsAddNextSemester(AccountingPeriod previousPeriod)
        {
            var newSemester = new AccountingPeriodBuilder(this.Strategy.Session)
                .WithPeriodNumber(previousPeriod.PeriodNumber + 1)
                .WithFrequency(new TimeFrequencies(this.Strategy.Session).Semester)
                .WithFromDate(previousPeriod.FromDate.AddMonths(6).Date)
                .WithThroughDate(previousPeriod.FromDate.AddMonths(12).AddSeconds(-1).Date)
                .WithParent(previousPeriod.Parent)
                .Build();

            return newSemester;
        }
    }
}