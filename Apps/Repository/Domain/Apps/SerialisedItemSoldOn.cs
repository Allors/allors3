// <copyright file="SerialisedItemSoldOn.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using System;

    using Attributes;
    using static Workspaces;

    #region Allors
    [Id("5ce9dc9e-1868-484b-9a10-d0ddf2ee4075")]
    #endregion
    public partial class SerialisedItemSoldOn : UniquelyIdentifiable
    {
        #region inherited properties
        public Guid UniqueId { get; set; }

        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        #endregion

        #region Allors
        [Id("363568f8-b523-47d7-af5a-794eb6d8fb7f")]
        #endregion
        [Workspace(Default)]
        [Indexed]
        [Size(256)]
        public string Name { get; set; }

        #region inherited methods

        public void OnBuild() { }

        public void OnPostBuild() { }

        public void OnInit() { }

        public void OnPreDerive() { }

        public void OnDerive() { }

        public void OnPostDerive() { }

        #endregion
    }
}
