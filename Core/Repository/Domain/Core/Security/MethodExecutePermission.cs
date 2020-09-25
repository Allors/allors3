// <copyright file="Permission.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the Extent type.</summary>

namespace Allors.Repository
{
    using System;

    using Allors.Repository.Attributes;

    #region Allors
    [Id("2E839427-58D6-4567-B9AA-FBE6071590E3")]
    #endregion
    public partial class MethodExecutePermission : Permission
    {
        #region inherited properties
        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        public Guid ConcreteClassPointer { get; set; }
        #endregion

        #region Allors
        [Id("CB76C8B7-681E-450B-A3EC-95C32E1ED5B6")]
        [AssociationId("7A629E9D-D5FF-4CD8-AF15-2A378FC9CF9D")]
        [RoleId("C35DC424-E609-4155-9EFE-55358A27F14B")]
        [Indexed]
        #endregion
        [Required]
        public Guid MethodTypePointer { get; set; }

        #region inherited methods

        public void OnBuild() { }

        public void OnPostBuild() { }

        public void OnInit()
        {
        }

        public void OnPreDerive() { }

        public void OnDerive() { }

        public void OnPostDerive() { }

        public void Delete() { }

        #endregion
    }
}
