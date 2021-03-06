// <copyright file="Many2OneTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.OriginSession.SessionWorkspace
{
    using System.Threading.Tasks;
    using Allors.Workspace.Domain;
    using Allors.Workspace;
    using Xunit;
    using System;

    public abstract class OneToManyTests : Test
    {
        private Func<ISession, Task>[] pushes;

        private Func<Context>[] contextFactories;

        protected OneToManyTests(Fixture fixture) : base(fixture)
        {

        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await this.Login("administrator");

            this.pushes = new Func<ISession, Task>[]
            {
                (session) => Task.CompletedTask,
                async (session) => await session.PushToWorkspace(),
                async (session) => {await session.PushToWorkspace(); await session.PullFromWorkspace(); }
            };

            var singleSessionContext = new SingleSessionContext(this, "Single shared");
            var multipleSessionContext = new MultipleSessionContext(this, "Multiple shared");

            this.contextFactories = new Func<Context>[]
            {
                () => singleSessionContext,
                () => new SingleSessionContext(this, "Single"),
                () => multipleSessionContext,
                () => new MultipleSessionContext(this, "Multiple"),
            };
        }

        [Fact]
        public async void SetRole()
        {
            foreach (var push in this.pushes)
            {
                foreach (WorkspaceMode mode in Enum.GetValues(typeof(WorkspaceMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (session1, session2) = ctx;

                        var c1x_1 = ctx.Session1.Create<SessionC1>();
                        var c1y_2 = await ctx.Create<WorkspaceC1>(session2, mode);

                        c1x_1.ShouldNotBeNull(ctx, mode);
                        c1y_2.ShouldNotBeNull(ctx, mode);

                        await session2.PushToWorkspace();
                        await session1.PullFromWorkspace();

                        var c1y_1 = session1.Instantiate(c1y_2);

                        c1y_1.ShouldNotBeNull(ctx, mode);

                        c1x_1.AddSessionC1WorkspaceC1One2Many(c1y_1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);

                        await push(session1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);
                    }
                }
            }
        }

        [Fact]
        public async void SetRoleToNull()
        {
            foreach (var push in this.pushes)
            {
                foreach (WorkspaceMode mode in Enum.GetValues(typeof(WorkspaceMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (session1, session2) = ctx;

                        var c1x_1 = ctx.Session1.Create<SessionC1>();
                        var c1y_2 = await ctx.Create<WorkspaceC1>(session2, mode);

                        c1x_1.ShouldNotBeNull(ctx, mode);
                        c1y_2.ShouldNotBeNull(ctx, mode);

                        await session2.PushToWorkspace();
                        await session1.PullFromWorkspace();

                        var c1y_1 = session1.Instantiate(c1y_2);

                        c1y_1.ShouldNotBeNull(ctx, mode);

                        c1x_1.AddSessionC1WorkspaceC1One2Many(null);
                        Assert.Empty(c1x_1.SessionC1WorkspaceC1One2Manies);

                        c1x_1.AddSessionC1WorkspaceC1One2Many(c1y_1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);

                        await push(session1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);
                    }
                }
            }
        }

        [Fact]
        public async void RemoveRole()
        {
            foreach (var push in this.pushes)
            {
                foreach (WorkspaceMode mode in Enum.GetValues(typeof(WorkspaceMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (session1, session2) = ctx;

                        var c1x_1 = ctx.Session1.Create<SessionC1>();
                        var c1y_2 = await ctx.Create<WorkspaceC1>(session2, mode);

                        c1x_1.ShouldNotBeNull(ctx, mode);
                        c1y_2.ShouldNotBeNull(ctx, mode);

                        await session2.PushToWorkspace();
                        await session1.PullFromWorkspace();

                        var c1y_1 = session1.Instantiate(c1y_2);

                        c1y_1.ShouldNotBeNull(ctx, mode);

                        c1x_1.AddSessionC1WorkspaceC1One2Many(c1y_1);
                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);

                        c1x_1.RemoveSessionC1WorkspaceC1One2Many(c1y_1);
                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldNotContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldNotEqual(c1x_1, ctx, mode);

                        await push(session1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldNotContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldNotEqual(c1x_1, ctx, mode);
                    }
                }
            }
        }

        [Fact]
        public async void RemoveNullRole()
        {
            foreach (var push in this.pushes)
            {
                foreach (WorkspaceMode mode in Enum.GetValues(typeof(WorkspaceMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (session1, session2) = ctx;

                        var c1x_1 = ctx.Session1.Create<SessionC1>();
                        var c1y_2 = await ctx.Create<WorkspaceC1>(session2, mode);

                        c1x_1.ShouldNotBeNull(ctx, mode);
                        c1y_2.ShouldNotBeNull(ctx, mode);

                        await session2.PushToWorkspace();
                        await session1.PullFromWorkspace();

                        var c1y_1 = session1.Instantiate(c1y_2);

                        c1y_1.ShouldNotBeNull(ctx, mode);

                        c1x_1.AddSessionC1WorkspaceC1One2Many(c1y_1);
                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);

                        c1x_1.RemoveSessionC1WorkspaceC1One2Many(null);
                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldEqual(c1x_1, ctx, mode);

                        c1x_1.RemoveSessionC1WorkspaceC1One2Many(c1y_1);
                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldNotContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldNotEqual(c1x_1, ctx, mode);

                        await push(session1);

                        c1x_1.SessionC1WorkspaceC1One2Manies.ShouldNotContains(c1y_1, ctx, mode);
                        c1y_1.SessionC1WhereSessionC1WorkspaceC1One2Many.ShouldNotEqual(c1x_1, ctx, mode);
                    }
                }
            }
        }
    }
}
