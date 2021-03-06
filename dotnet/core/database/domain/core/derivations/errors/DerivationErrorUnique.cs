// <copyright file="DerivationErrorUnique.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Errors
{
    using Database.Derivations;
    using Meta;
    using Resources;

    public class DerivationErrorUnique : DerivationError
    {
        public DerivationErrorUnique(IValidation validation, IDerivationRelation relation)
            : base(validation, new[] { relation }, DomainErrors.DerivationErrorUnique)
        {
        }

        public DerivationErrorUnique(IValidation validation, IObject association, IRoleType roleType) :
            this(validation, new DerivationRelation(association, roleType))
        {
        }
    }
}
