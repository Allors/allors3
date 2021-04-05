// <copyright file="ChangedRoles.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDomainDerivation type.</summary>

namespace Allors.Database.Derivations
{
    using Meta;

    public class AssociationPattern : Pattern
    {
        public AssociationPattern(IComposite objectType, IAssociationType associationType) : base(objectType) => this.AssociationType = associationType;

        public AssociationPattern(IAssociationType associationType) => this.AssociationType = associationType;

        public AssociationPattern(IRoleType roleType) : this(roleType.AssociationType) { }

        public IAssociationType AssociationType { get; }
    }
}
