// <copyright file="TestNoTreeController.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Server.Controllers
{
    using Allors.Domain;
    using Server;
    using Allors.Services;
    using Api.Json.Pull;
    using Microsoft.AspNetCore.Mvc;

    public class TestNoTreeController : Controller
    {
        public TestNoTreeController(ISessionService sessionService)
        {
            this.Session = sessionService.Session;
            this.TreeService = this.Session.Database.Scope().TreeService;
        }

        private ISession Session { get; }

        public ITreeService TreeService { get; }


        [HttpPost]
        public IActionResult Pull()
        {
            var acls = new WorkspaceAccessControlLists(this.Session.Scope().User);
            var response = new PullResponseBuilder(acls, this.TreeService);
            response.AddObject("object", acls.User);
            response.AddCollection("collection", new Organisations(this.Session).Extent());
            return this.Ok(response.Build());
        }
    }
}
