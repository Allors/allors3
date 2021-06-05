// <copyright file="Organisation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the Person type.</summary>

namespace Allors.Database.Domain
{
    public partial class Organisation
    {
        public void CustomOnPostDerive(ObjectOnPostDerive _) => this.PostDeriveTrigger = true;

        public void CustomToggleCanWrite(OrganisationToggleCanWrite method)
        {
            if (this.DeniedPermissions.Count != 0)
            {
                this.RemoveDeniedPermissions();
            }
            else
            {
                var permissions = new Permissions(this.strategy.Transaction);
                var deniedPermissions = new[]
                                            {
                                                permissions.Get(this.Meta, this.Meta.Name, Operations.Write),
                                                permissions.Get(this.Meta, this.Meta.Owner, Operations.Write),
                                                permissions.Get(this.Meta, this.Meta.Employees, Operations.Write),
                                            };

                this.DeniedPermissions = deniedPermissions;
            }
        }

        public void CustomJustDoIt(OrganisationJustDoIt _) => this.JustDidIt = true;

        public override string ToString() => this.Name;
    }
}