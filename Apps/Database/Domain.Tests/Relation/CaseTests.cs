// <copyright file="CaseTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class CaseTests : DomainTest, IClassFixture<Fixture>
    {
        public CaseTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenCase_WhenBuild_ThenLastObjectStateEqualsCurrencObjectState()
        {
            var complaint = new CaseBuilder(this.Session).WithDescription("Complaint").Build();

            this.Session.Derive();

            Assert.Equal(new CaseStates(this.Session).Opened, complaint.CaseState);
            Assert.Equal(complaint.LastCaseState, complaint.CaseState);
        }

        [Fact]
        public void GivenCase_WhenBuild_ThenPreviousObjectStateIsNull()
        {
            var complaint = new CaseBuilder(this.Session).WithDescription("Complaint").Build();

            this.Session.Derive();

            Assert.Null(complaint.PreviousCaseState);
        }

        [Fact]
        public void GivenCase_WhenConfirmed_ThenCurrentCaseStatusMustBeDerived()
        {
            var complaint = new CaseBuilder(this.Session).WithDescription("Complaint").Build();

            this.Session.Derive();

            Assert.Single(complaint.AllVersions);
            Assert.Equal(new CaseStates(this.Session).Opened, complaint.CaseState);

            complaint.AppsClose();

            this.Session.Derive();

            Assert.Equal(2, complaint.AllVersions.Count);
            Assert.Equal(new CaseStates(this.Session).Closed, complaint.CaseState);
        }
    }
}
