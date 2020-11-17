// <copyright file="ObjectTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Workspace.Local
{
    public class LifecycleTests : Workspace.LifecycleTests
    {
        public LifecycleTests() => this.Profile = new Profile();

        protected override IProfile Profile { get; }
    }
}