// <copyright file="RelationExtentTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Adapters.SqlClient
{
    using Adapters;
    using System;
    using Xunit;

    public class RelationExtentTest : Adapters.RelationExtentTest, IClassFixture<Fixture<RelationExtentTest>>
    {
        private readonly Profile profile;

        public RelationExtentTest() => this.profile = new Profile(this.GetType().Name);

        protected override IProfile Profile => this.profile;

        public override void Dispose() => this.profile.Dispose();
    }
}
