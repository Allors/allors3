// <copyright file="One2ManyTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Adapters.Memory
{
    using System;

    using Allors.Adapters;

    public class One2ManyTest : Adapters.One2ManyTest, IDisposable
    {
        private readonly Profile profile = new Profile();

        protected override IProfile Profile => this.profile;

        public override void Dispose() => this.profile.Dispose();
    }
}
