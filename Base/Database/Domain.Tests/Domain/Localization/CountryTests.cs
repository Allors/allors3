// <copyright file="CountryTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Tests
{
    using Allors.Database.Domain;
    using Xunit;

    public class CountryTests : DomainTest, IClassFixture<Fixture>
    {
        public CountryTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenCountryWhenValidatingThenRequiredRelationsMustExist()
        {
            var builder = new CountryBuilder(this.Session);
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            builder.WithIsoCode("XX").Build();
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            this.Session.Rollback();

            builder.WithName("X Country");

            builder.Build();

            Assert.False(this.Session.Derive(false).HasErrors);

            this.Session.Rollback();

            builder = new CountryBuilder(this.Session);
            builder.WithName("X Country");

            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);
        }
    }
}
