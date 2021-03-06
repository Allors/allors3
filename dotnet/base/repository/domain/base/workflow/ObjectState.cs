// <copyright file="ObjectState.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the Extent type.</summary>

namespace Allors.Repository
{
    using Attributes;
    using static Workspaces;


    #region Allors
    [Id("f991813f-3146-4431-96d0-554aa2186887")]
    #endregion
    public partial interface ObjectState : UniquelyIdentifiable
    {
        #region Allors
        [Id("59338f0b-40e7-49e8-ba1a-3ecebf96aebe")]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Indexed]
        Permission[] ObjectDeniedPermissions { get; set; }

        #region Allors
        [Id("b86f9e42-fe10-4302-ab7c-6c6c7d357c39")]
        #endregion
        [Workspace(Default)]
        [Indexed]
        [Size(256)]
        string Name { get; set; }
    }
}
