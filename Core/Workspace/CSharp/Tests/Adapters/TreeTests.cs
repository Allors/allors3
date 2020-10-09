// <copyright file="PullTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Adapters
{
    using System.Linq;
    using Allors.Workspace.Data;
    using Allors.Workspace.Domain;
    using Allors.Workspace.Meta;
    using Xunit;
    using Result = Allors.Workspace.Data.Result;

    public class TreeTests : Test
    {
        [Fact]
        public async void Users()
        {
            var context = this.Workspace.CreateSession();

            var pull = new Pull
            {
                Extent = new Extent(M.User.ObjectType),
                Results = new[]
                {
                    new Result
                    {
                        Fetch = new Fetch
                        {
                            Include = new UserNodeBuilder(M,v => v.Person_Address()),
                        },
                    },
                },
            };

            var result = await context.Load(pull);

            var users = result.GetCollection<User>();

            var personWithAddress = (Person)users.Single(v => (v as Person)?.ExistAddress == true);

            Assert.NotNull(personWithAddress);
            Assert.Equal("Jane", personWithAddress.FirstName);
        }
    }
}
