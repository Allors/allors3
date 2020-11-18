// <copyright file="IUnitOfMeasureTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class IUnitOfMeasureTests : DomainTest, IClassFixture<Fixture>
    {
        public IUnitOfMeasureTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenUnitOfMeasure_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var builder = new UnitOfMeasureBuilder(this.Session);
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            this.Session.Rollback();

            builder.WithName("Mt");

            Assert.False(this.Session.Derive(false).HasErrors);
        }
    }
}
