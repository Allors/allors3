// <copyright file="SqlClientTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Adapters.SqlClient.Snapshot
{
    using System;
    using Allors.Adapters;

    public class SqlClientTest : SqlClient.SqlClientTest, IDisposable
    {
        private readonly Profile profile = new Profile();

        protected override IProfile Profile => this.profile;

        public void Dispose() => this.profile.Dispose();
    }
}