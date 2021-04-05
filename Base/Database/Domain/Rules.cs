// <copyright file="ObjectsBase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Meta;

    public static partial class Rules
    {
        public static Rule[] Create(MetaPopulation m) =>
            new Rule[]
            {
                // Core
                new MediaRule(m),
                new TransitionalDeniedPermissionRule(m),

                // Custom
                new PersonFullNameDerivation(m),
                new PersonGreetingDerivation(m),
            };
    }
}
