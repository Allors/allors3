// <copyright file="DerivationService.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Services
{
    using System;
    using Domain;
    using Domain.Derivations.Default;

    public class DerivationService : IDerivationService
    {
        public Func<ISession, IDerivation> Factory { get; set; }

        public IDerivation CreateDerivation(ISession session) => this.Factory != null ? this.Factory(session) : new Derivation(session);
    }
}
