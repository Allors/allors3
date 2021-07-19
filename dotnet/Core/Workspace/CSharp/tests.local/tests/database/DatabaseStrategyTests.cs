// <copyright file="Many2OneTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.OriginDatabase.Local
{
    using Xunit;

    public class DatabaseStrategyTests : OriginDatabase.DatabaseStrategyTests, IClassFixture<Fixture>
    {
        public DatabaseStrategyTests(Fixture fixture) : base(fixture) => this.Profile = new Workspace.Local.Profile(fixture);

        public override IProfile Profile { get; }
    }
}
