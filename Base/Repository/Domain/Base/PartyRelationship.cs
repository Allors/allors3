// <copyright file="PartyRelationship.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using Allors.Repository.Attributes;
    using static Workspaces;

    #region Allors
    [Id("084abb92-31fd-46e6-ab85-9a7a88c9d72b")]
    #endregion
    public partial interface PartyRelationship : Period, Deletable, Object
    {
        #region Allors
        [Id("8472a037-3a42-4d1c-a7cb-f8866141f65d")]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Derived]
        [Indexed]
        [Workspace(Default)]
        Party[] Parties { get; set; }

        #region Allors
        [Id("6dfbbbe2-31a1-4182-8329-0ba7989f5a71")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        Agreement[] Agreements { get; set; }
    }
}
