// <copyright file="GeneralLedgerAccountClassificationTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class GeneralLedgerAccountClassificationTests : DomainTest, IClassFixture<Fixture>
    {
        public GeneralLedgerAccountClassificationTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenGeneralLedgerAccountClassification_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var builder = new GeneralLedgerAccountClassificationBuilder(this.Transaction);
            builder.Build();

            Assert.True(this.Transaction.Derive(false).HasErrors);

            this.Transaction.Rollback();

            builder.WithName("GeneralLedgerAccountClassification");
            builder.Build();

            Assert.False(this.Transaction.Derive(false).HasErrors);
        }
    }
}
