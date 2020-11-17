// <copyright file="DefaultDerivationFactory.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.State
{
    using Domain;
    using Domain.Derivations.Default;

    public class DefaultDerivationFactory : IDerivationFactory
    {
        public IDerivation CreateDerivation(ISession session) => new Derivation(session);
    }
}