// <copyright file="ObjectTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace
{
    using System.Collections.Generic;
    using System.Net.Http;
    using Xunit;

    public abstract class ObjectTests : Test
    {
        protected ObjectTests(Fixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void NonExistingPullService()
        {
            await this.Login("administrator");

            var session = this.Workspace.CreateSession();

            var exceptionThrown = false;
            try
            {
                await session.Load(
                    "ThisIsWrong",
                    new Dictionary<string, object> { { "step", 0 } });
            }
            catch (HttpRequestException)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown);
        }
    }
}
