// <copyright file="RemoteTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests.Local
{
    using System;
    using System.Net.Http;

    using Allors.Workspace;
    using Allors.Workspace.Remote;
    using Allors.Workspace.Domain;
    using Allors.Workspace.Meta;

    using Xunit;

    [Collection("Local")]
    public class LocalTest : IDisposable
    {
        public const string Url = "http://localhost:5000";

        public const string InitUrl = "/Test/Init";
        public const string SetupUrl = "/Test/Setup";
        public const string LoginUrl = "/Test/Login";

        public Workspace Workspace { get; set; }

        public RemoteDatabase Database { get; set; }

        public LocalTest()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(Url),
            };

            this.Database = new RemoteDatabase(client);

            var objectFactory = new ObjectFactory(MetaPopulation.Instance, typeof(User));
            this.Workspace = new Workspace(objectFactory);

            this.Init();
        }

        public void Dispose()
        {
        }

        private void Init()
        {
            var init = this.Database.HttpClient.GetAsync(SetupUrl).Result;

            var user = "administrator";
            var uri = new Uri("/TestAuthentication/Token", UriKind.Relative);
            var loggedIn = this.Database.Login(uri, user, null).Result;
        }
    }
}
