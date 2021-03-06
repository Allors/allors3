// <copyright file="CurrencyTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the CurrencyTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Domain;
    using Xunit;

    public class CurrencyTests : DomainTest, IClassFixture<Fixture>
    {
        public CurrencyTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenCurrencyWhenValidatingThenRequiredRelationsMustExist()
        {
            var builder = new CurrencyBuilder(this.Session);
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            builder.WithIsoCode("BND").Build();

            Assert.True(this.Session.Derive(false).HasErrors);

            var locales = new Locales(this.Session).Extent().ToArray();
            var locale = new Locales(this.Session).FindBy(this.M.Locale.Name, Locales.EnglishGreatBritainName);

            builder
                .WithLocalisedName(
                    new LocalisedTextBuilder(this.Session)
                    .WithText("Brunei Dollar")
                    .WithLocale(locale)
                .Build());

            Assert.False(this.Session.Derive(false).HasErrors);
        }
    }
}
