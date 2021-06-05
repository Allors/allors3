// <copyright file="MethodTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace
{
    using System.Linq;
    using Allors.Workspace;
    using Allors.Workspace.Data;
    using Allors.Workspace.Domain;
    using Xunit;

    public abstract class MethodTests : Test
    {
        protected MethodTests(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void CallSingle()
        {
            await this.Login("administrator");

            var session = this.Workspace.CreateSession();

            var pull = new[] { new Pull { Extent = new Extent(this.M.Organisation) } };

            var organisation = (await session.Pull(pull)).GetCollection<Organisation>().First();

            Assert.False(organisation.JustDidIt);

            var invokeResult = await session.Invoke(organisation.JustDoIt);

            Assert.False(invokeResult.HasErrors);

            await session.Pull(new Pull { Object = organisation });

            Assert.True(organisation.JustDidIt);
            Assert.True(organisation.JustDidItDerived);
        }

        [Fact]
        public async void CallMultiple()
        {
            await this.Login("administrator");

            var session = this.Workspace.CreateSession();

            var pull = new[] { new Pull { Extent = new Extent(this.M.Organisation) } };

            var organisation1 = (await session.Pull(pull)).GetCollection<Organisation>().First();
            var organisation2 = (await session.Pull(pull)).GetCollection<Organisation>().Skip(1).First();

            Assert.False(organisation1.JustDidIt);

            var invokeResult = await session.Invoke(new[] { organisation1.JustDoIt, organisation2.JustDoIt });

            Assert.False(invokeResult.HasErrors);

            await session.Pull(pull);

            Assert.True(organisation1.JustDidIt);
            Assert.True(organisation1.JustDidItDerived);

            Assert.True(organisation2.JustDidIt);
            Assert.True(organisation2.JustDidItDerived);
        }

        [Fact]
        public async void CallMultipleIsolated()
        {
            await this.Login("administrator");

            var session = this.Workspace.CreateSession();

            var pull = new[] { new Pull { Extent = new Extent(this.M.Organisation) } };

            var organisation1 = (await session.Pull(pull)).GetCollection<Organisation>().First();
            var organisation2 = (await session.Pull(pull)).GetCollection<Organisation>().Skip(1).First();

            Assert.False(organisation1.JustDidIt);

            var invokeResult = await session.Invoke(new[] { organisation1.JustDoIt, organisation2.JustDoIt }, new InvokeOptions { Isolated = true });

            Assert.False(invokeResult.HasErrors);

            await session.Pull(pull);

            Assert.True(organisation1.JustDidIt);
            Assert.True(organisation1.JustDidItDerived);

            Assert.True(organisation2.JustDidIt);
            Assert.True(organisation2.JustDidItDerived);
        }

    }
}