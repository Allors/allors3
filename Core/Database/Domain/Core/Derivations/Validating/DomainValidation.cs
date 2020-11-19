// <copyright file="IDomainChangeSet.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Validating
{
    using Errors;
    using Database.Derivations;

    public class DomainValidation : IDomainValidation
    {
        private IValidation Validation { get; }

        public DomainValidation(IValidation validation) => this.Validation = validation;

        public void AddError(string error) => this.Validation.AddError(new DerivationErrorGeneric(this.Validation, relation: null, error));
    }
}
