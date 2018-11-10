// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegalForms.cs" company="Allors bvba">
//   Copyright 2002-2012 Allors bvba.
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Allors.Domain
{
    using Allors.Meta;

    public partial class InternalOrganisations
    {
        protected override void AppsPrepare(Setup config)
        {
            base.BasePrepare(config);

            config.AddDependency(this.ObjectType, M.TemplateType.ObjectType);
            config.AddDependency(this.ObjectType, M.ShipmentMethod.ObjectType);
            config.AddDependency(this.ObjectType, M.Carrier.ObjectType);
            config.AddDependency(this.ObjectType, M.BillingProcess.ObjectType);
        }
    }
}