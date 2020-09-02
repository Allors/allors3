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

    public class OrganisationRollupDerivation : IDomainDerivation
    {
        public Guid Id => new Guid("F34E1F40-B1DD-4F0D-A87C-78F44ACF8512");

        public IEnumerable<Pattern> Patterns { get; } = new Pattern[]
        {
            new CreatedPattern(M.OrganisationRollUp.Class),
        };

        public void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var organisationRollUp in matches.Cast<OrganisationRollUp>())
            {
                organisationRollUp.Parties = new Party[] { organisationRollUp.Child, organisationRollUp.Parent };

                if (!organisationRollUp.ExistParent | !organisationRollUp.ExistChild)
                {
                    // TODO: Move Delete
                    organisationRollUp.Delete();
                }
            }
        }
    }
}
