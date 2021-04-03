// <copyright file="DerivationErrorNotAllowed.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//
// </summary>

namespace Allors.Database.Domain.Derivations.Errors
{
    using Meta;
    using Resources;

    public class DerivationErrorNotAllowed : DerivationError
    {
        public DerivationErrorNotAllowed(IValidation validation, DerivationRelation relation)
            : base(validation, new[] { relation }, DomainErrors.DerivationErrorNotAllowed)
        {
        }

        public DerivationErrorNotAllowed(IValidation validation, IObject association, IRoleType roleType) :
            this(validation, new DerivationRelation(association, roleType))
        {
        }
    }
}
