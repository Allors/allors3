// <copyright file="DerivationErrorEquals.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Errors
{
    using Database.Derivations;
    using Resources;

    public class DerivationErrorEquals : DerivationError
    {
        public DerivationErrorEquals(IValidation validation, DerivationRelation[] derivationRelations)
            : base(validation, derivationRelations, DomainErrors.DerivationErrorEquals)
        {
        }
    }
}
