// <copyright file="IDerivationFactory.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Database;
    using Derivations;

    public interface IDerivationFactory
    {
        IDerivation CreateDerivation(ITransaction transaction, bool continueOnError = false);
    }
}
