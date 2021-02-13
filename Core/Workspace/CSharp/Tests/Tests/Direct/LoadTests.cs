// <copyright file="PullTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.Direct
{
    using Xunit;

    public class LoadTests : Workspace.LoadTests, IClassFixture<Fixture>
    {
        public LoadTests(Fixture fixture) : base(fixture) => this.Profile = new Profile(fixture);

        protected override IProfile Profile { get; }
    }
}