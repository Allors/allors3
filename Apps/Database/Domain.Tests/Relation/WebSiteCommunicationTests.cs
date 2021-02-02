// <copyright file="WebSiteCommunicationTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class WebSiteCommunicationTests : DomainTest, IClassFixture<Fixture>
    {
        public WebSiteCommunicationTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenWebSiteCommunication_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var person = new PersonBuilder(this.Session).WithLastName("person").Build();

            this.Session.Derive();
            this.Session.Commit();

            var builder = new WebSiteCommunicationBuilder(this.Session).WithFromParty(person).WithToParty(person);
            var communication = builder.Build();

            var validation = this.Session.Derive(false);

            Assert.True(validation.HasErrors);

            this.Session.Rollback();

            builder.WithSubject("Website communication");
            communication = builder.Build();

            validation = this.Session.Derive(false);

            Assert.False(validation.HasErrors);

            Assert.Equal(communication.CommunicationEventState, new CommunicationEventStates(this.Session).Scheduled);
            Assert.Equal(communication.CommunicationEventState, communication.LastCommunicationEventState);
        }

        [Fact]
        public void GivenWebSiteCommunication_WhenDeriving_ThenInvolvedPartiesAreDerived()
        {
            var owner = new PersonBuilder(this.Session).WithLastName("owner").Build();
            var originator = new PersonBuilder(this.Session).WithLastName("originator").Build();
            var receiver = new PersonBuilder(this.Session).WithLastName("receiver").Build();

            this.Session.Derive();
            this.Session.Commit();

            var communication = new WebSiteCommunicationBuilder(this.Session)
                .WithSubject("Hello world!")
                .WithOwner(owner)
                .WithFromParty(originator)
                .WithToParty(receiver)
                .Build();

            this.Session.Derive();

            Assert.Equal(3, communication.InvolvedParties.Count);
            Assert.Contains(owner, communication.InvolvedParties);
            Assert.Contains(originator, communication.InvolvedParties);
            Assert.Contains(receiver, communication.InvolvedParties);
        }

        [Fact]
        public void GivenWebSiteCommunication_WhenOriginatorIsDeleted_ThenCommunicationEventIsDeleted()
        {
            var owner = new PersonBuilder(this.Session).WithLastName("owner").Build();
            var originator = new PersonBuilder(this.Session).WithLastName("originator").Build();
            var receiver = new PersonBuilder(this.Session).WithLastName("receiver").Build();

            this.Session.Derive();
            this.Session.Commit();

            new WebSiteCommunicationBuilder(this.Session)
                .WithSubject("Hello world!")
                .WithOwner(owner)
                .WithFromParty(originator)
                .WithToParty(receiver)
                .Build();

            this.Session.Derive();

            Assert.Single(this.Session.Extent<WebSiteCommunication>());

            originator.Delete();
            this.Session.Derive();

            Assert.Empty(this.Session.Extent<WebSiteCommunication>());
        }
    }

    public class WebSiteCommunicationDerivationTests : DomainTest, IClassFixture<Fixture>
    {
        public WebSiteCommunicationDerivationTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void ChangedSubjectDeriveWorkItemDescription()
        {
            var communication = new WebSiteCommunicationBuilder(this.Session).Build();
            this.Session.Derive(false);

            communication.Subject = "subject";
            this.Session.Derive(false);

            Assert.Contains("subject", communication.WorkItemDescription);
        }

        [Fact]
        public void ChangedToPartyDeriveWorkItemDescription()
        {
            var communication = new WebSiteCommunicationBuilder(this.Session).Build();
            this.Session.Derive(false);

            var person = new PersonBuilder(this.Session).WithLastName("person").Build();
            this.Session.Derive(false);

            communication.ToParty = person;
            this.Session.Derive(false);

            Assert.Contains("person", communication.WorkItemDescription);
        }

        [Fact]
        public void ChangedPartyPartyNameDeriveWorkItemDescription()
        {
            var person = new PersonBuilder(this.Session).WithLastName("person").Build();
            this.Session.Derive(false);

            var communication = new WebSiteCommunicationBuilder(this.Session).WithToParty(person).Build();
            this.Session.Derive(false);

            person.LastName = "changed";
            this.Session.Derive(false);

            Assert.Contains("changed", communication.WorkItemDescription);
        }
    }
}
