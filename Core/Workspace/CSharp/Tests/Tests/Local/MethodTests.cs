// <copyright file="ObjectTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.Local
{
    public class MethodTests : Workspace.MethodTests
    {
        public MethodTests() => this.Profile = new Profile();

        protected override IProfile Profile { get; }
    }
}
