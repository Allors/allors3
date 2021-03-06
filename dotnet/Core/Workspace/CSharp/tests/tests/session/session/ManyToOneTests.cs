// <copyright file="Many2OneTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.OriginSession.SessionSession
{
    using System.Threading.Tasks;
    using Allors.Workspace.Domain;
    using Allors.Workspace;
    using Xunit;
    using System;

    public abstract class ManyToOneTests : Test
    {
        private Func<ISession, Task>[] pushes;

        private Func<Context>[] contextFactories;

        protected ManyToOneTests(Fixture fixture) : base(fixture)
        {

        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await this.Login("administrator");

            this.pushes = new Func<ISession, Task>[]
            {
                (session) => Task.CompletedTask,
                async (session) => await session.Push()
            };

            var multipleSessionContext = new MultipleSessionContext(this, "Multiple shared");

            this.contextFactories = new Func<Context>[]
            {
                () => multipleSessionContext,
                () => new MultipleSessionContext(this, "Multiple"),
            };
        }

        [Fact]
        public async void SetRole()
        {
            foreach (var push in this.pushes)
            {
                foreach (var contextFactory in this.contextFactories)
                {
                    var ctx = contextFactory();
                    var (session1, session2) = ctx;

                    var c1x_1 = ctx.Session1.Create<SessionC1>();
                    var c1y_2 = ctx.Session1.Create<SessionC1>();

                    c1x_1.ShouldNotBeNull(ctx);
                    c1y_2.ShouldNotBeNull(ctx);

                    await session1.Push();

                    c1x_1.SessionC1SessionC1Many2One = c1y_2;

                    c1x_1.SessionC1SessionC1Many2One.ShouldEqual(c1y_2, ctx);
                    c1y_2.SessionC1sWhereSessionC1SessionC1Many2One.ShouldContains(c1x_1, ctx);

                    await push(session1);

                    c1x_1.SessionC1SessionC1Many2One.ShouldEqual(c1y_2, ctx);
                    c1y_2.SessionC1sWhereSessionC1SessionC1Many2One.ShouldContains(c1x_1, ctx);
                }
            }
        }

        [Fact]
        public async void RemoveRole()
        {
            foreach (var push1 in this.pushes)
            {
                foreach (var contextFactory in this.contextFactories)
                {
                    var ctx = contextFactory();
                    var (session1, session2) = ctx;

                    var c1x_1 = ctx.Session1.Create<SessionC1>();
                    var c1y_2 = ctx.Session1.Create<SessionC1>();

                    c1x_1.ShouldNotBeNull(ctx);
                    c1y_2.ShouldNotBeNull(ctx);

                    await session1.Push();

                    c1x_1.SessionC1SessionC1Many2One = c1y_2;
                    c1x_1.SessionC1SessionC1Many2One.ShouldEqual(c1y_2, ctx);
                    c1y_2.SessionC1sWhereSessionC1SessionC1Many2One.ShouldContains(c1x_1, ctx);

                    c1x_1.RemoveSessionC1SessionC1Many2One();
                    c1x_1.SessionC1SessionC1Many2One.ShouldNotEqual(c1y_2, ctx);
                    c1y_2.SessionC1sWhereSessionC1SessionC1Many2One.ShouldNotContains(c1x_1, ctx);

                    await push1(session1);

                    c1x_1.SessionC1SessionC1Many2One.ShouldNotEqual(c1y_2, ctx);
                    c1y_2.SessionC1sWhereSessionC1SessionC1Many2One.ShouldNotContains(c1x_1, ctx);
                }
            }
        }
    }
}
