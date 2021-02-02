// <copyright file="PhoneCommunicationTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the MediaTests type.</summary>

namespace Allors.Database.Domain.Tests
{
    using Xunit;

    public class PhoneCommunicationTests : DomainTest, IClassFixture<Fixture>
    {
        public PhoneCommunicationTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void GivenPhoneCommunication_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var receiver = new PersonBuilder(this.Session).WithLastName("receiver").Build();
            var caller = new PersonBuilder(this.Session).WithLastName("caller").Build();

            this.Session.Derive();
            this.Session.Commit();

            var builder = new PhoneCommunicationBuilder(this.Session);
            builder.WithToParty(receiver);
            builder.WithFromParty(caller);
            builder.Build();

            Assert.True(this.Session.Derive(false).HasErrors);
            this.Session.Rollback();

            builder.WithSubject("Phonecall");
            var communication = builder.Build();

            Assert.False(this.Session.Derive(false).HasErrors);

            Assert.Equal(communication.CommunicationEventState, new CommunicationEventStates(this.Session).Scheduled);
            Assert.Equal(communication.CommunicationEventState, communication.LastCommunicationEventState);
        }

        [Fact]
        public void GivenPhoneCommunicationIsBuild_WhenDeriving_ThenStatusIsSet()
        {
            var communication = new PhoneCommunicationBuilder(this.Session)
                .WithSubject("Hello world!")
                .WithOwner(new PersonBuilder(this.Session).WithLastName("owner").Build())
                .WithFromParty(new PersonBuilder(this.Session).WithLastName("caller").Build())
                .WithToParty(new PersonBuilder(this.Session).WithLastName("receiver").Build())
                .Build();

            Assert.False(this.Session.Derive(false).HasErrors);

            Assert.Equal(communication.CommunicationEventState, new CommunicationEventStates(this.Session).Scheduled);
            Assert.Equal(communication.CommunicationEventState, communication.LastCommunicationEventState);
        }

        [Fact]
        public void GivenPhoneCommunication_WhenDeriving_ThenInvolvedPartiesAreDerived()
        {
            var owner = new PersonBuilder(this.Session).WithLastName("owner").Build();
            var caller = new PersonBuilder(this.Session).WithLastName("caller").Build();
            var receiver = new PersonBuilder(this.Session).WithLastName("receiver").Build();

            this.Session.Derive();
            this.Session.Commit();

            var communication = new PhoneCommunicationBuilder(this.Session)
                .WithSubject("Hello world!")
                .WithOwner(owner)
                .WithFromParty(caller)
                .WithToParty(receiver)
                .Build();

            this.Session.Derive();

            Assert.Equal(3, communication.InvolvedParties.Count);
            Assert.Contains(owner, communication.InvolvedParties);
            Assert.Contains(caller, communication.InvolvedParties);
            Assert.Contains(receiver, communication.InvolvedParties);
        }

        [Fact]
        public void GivenPhoneCommunication_WhenCallerIsDeleted_ThenCommunicationEventIsDeleted()
        {
            var owner = new PersonBuilder(this.Session).WithLastName("owner").Build();
            var originator = new PersonBuilder(this.Session).WithLastName("originator").Build();
            var receiver = new PersonBuilder(this.Session).WithLastName("receiver").Build();

            this.Session.Derive();
            this.Session.Commit();

            new PhoneCommunicationBuilder(this.Session)
                .WithSubject("Hello world!")
                .WithOwner(owner)
                .WithFromParty(originator)
                .WithToParty(receiver)
                .Build();

            this.Session.Derive();

            Assert.Single(this.Session.Extent<PhoneCommunication>());

            originator.Delete();
            this.Session.Derive();

            Assert.Empty(this.Session.Extent<PhoneCommunication>());
        }
    }

    public class PhoneCommunicationDerivationTests : DomainTest, IClassFixture<Fixture>
    {
        public PhoneCommunicationDerivationTests(Fixture fixture) : base(fixture) { }

        [Fact]
        public void ChangedSubjectDeriveWorkItemDescription()
        {
            var communication = new PhoneCommunicationBuilder(this.Session).Build();
            this.Session.Derive(false);

            communication.Subject = "subject";
            this.Session.Derive(false);

            Assert.Contains("subject", communication.WorkItemDescription);
        }

        [Fact]
        public void ChangedToPartyDeriveWorkItemDescription()
        {
            var communication = new PhoneCommunicationBuilder(this.Session).Build();
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

            var communication = new PhoneCommunicationBuilder(this.Session).WithToParty(person).Build();
            this.Session.Derive(false);

            person.LastName = "changed";
            this.Session.Derive(false);

            Assert.Contains("changed", communication.WorkItemDescription);
        }
    }

}
