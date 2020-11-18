// <copyright file="Organisation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Meta
{
    public partial class MetaBuilder
    {
        private void BuildBase(MetaPopulation meta, Domains domains, ObjectTypes objectTypes, RelationTypes relationTypes, MethodTypes methodTypes, RoleClasses roleClasses)
        {
            roleClasses.LocalisedTextLocale.IsRequired = true;
        }
    }
}
