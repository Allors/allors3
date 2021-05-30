// <copyright file="DerivationErrorRequired.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//
// </summary>

namespace Allors.Database.Domain.Derivations.Errors
{
    using Database.Derivations;
    using Derivations.Rules;
    using Meta;
    using Resources;

    public class DerivationErrorRequired : DerivationError
    {
        public DerivationErrorRequired(IValidation validation, DerivationRelation relation)
            : base(validation, new[] { relation }, DomainErrors.DerivationErrorRequired)
        {
        }

        public DerivationErrorRequired(IValidation validation, IObject association, IRoleType roleType) :
            this(validation, new DerivationRelation(association, roleType))
        {
        }

        public DerivationErrorRequired(IValidation validation, IObject role, IAssociationType associationType) :
            this(validation, new DerivationRelation(role, associationType))
        {
        }
    }
}
