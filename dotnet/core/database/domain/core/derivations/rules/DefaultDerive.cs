// <copyright file="DefaultDerive.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Default
{
    using Database.Derivations;

    public class DefaultDerive : IDerive
    {
        private readonly ITransaction transaction;

        public DefaultDerive(ITransaction transaction) => this.transaction = transaction;

        public IDerivationResult Derive() => this.transaction.Derive();
    }
}
