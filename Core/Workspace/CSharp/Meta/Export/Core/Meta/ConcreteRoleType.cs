//------------------------------------------------------------------------------------------------- 
// <copyright file="ConcreteRoleType.cs" company="Allors bvba">
// Copyright 2002-2017 Allors bvba.
// 
// Dual Licensed under
//   a) the Lesser General Public Licence v3 (LGPL)
//   b) the Allors License
// 
// The LGPL License is included in the file lgpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>Defines the IObjectType type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Allors.Workspace.Meta
{
    public sealed partial class ConcreteRoleType : IConcreteRoleType
    {
        public ConcreteRoleType(Class @class, RoleType roleType)
        {
            this.Class = @class;
            this.RoleType = roleType;
        }

        public bool IsRequired => this.IsRequiredOverride ?? ((RoleType)this.RoleType).IsRequired;

        public bool? IsRequiredOverride { get; set; }

        public bool IsUnique => this.IsUniqueOverride ?? ((RoleType)this.RoleType).IsUnique;

        public bool? IsUniqueOverride { get; set; }

        public IRoleType RoleType { get; set; }

        public IRelationType RelationType => this.RoleType.RelationType;

        public Class Class { get; }
        
        public static implicit operator RoleType(ConcreteRoleType concreteRoleType)
        {
            return (RoleType)concreteRoleType.RoleType;
        }
    }
}