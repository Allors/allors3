// <copyright file="ShipmentPackageTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class ShipmentPackageTests : DomainTest, IClassFixture<Fixture>
    {
        public ShipmentPackageTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenShipmentPackageBuilder_WhenBuild_ThenPostBuildRelationsMustExist()
        {
            var package = new ShipmentPackageBuilder(this.Session).Build();

            Assert.True(package.ExistCreationDate);
        }
    }
}
