// <copyright file="Company.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using Allors;

    public partial class Company
    {
        public static Company Create(ISession session, string name)
        {
            var company = Create(session);
            company.Name = name;
            return company;
        }

        public static Company Create(ISession session, string name, int index)
        {
            var company = Create(session);
            company.Name = name;
            company.Index = index;
            return company;
        }

        public override string ToString() => this.Name;
    }
}
