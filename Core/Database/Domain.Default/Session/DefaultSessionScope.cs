// <copyright file="DefaultSessionScope.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the DomainTest type.</summary>

namespace Allors
{
    using System.Linq;
    using System.Security.Claims;
    using Domain;
    using Microsoft.AspNetCore.Http;

    public partial class DefaultSessionScope : SessionScope
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public DefaultSessionScope(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;

        public override void OnInit(ISession session)
        {
            var nameIdentifier = this.httpContextAccessor?.HttpContext.User.Claims
                .FirstOrDefault(v => v.Type == ClaimTypes.NameIdentifier)
                ?.Value;

            if (long.TryParse(nameIdentifier, out var userId))
            {
                this.User = (User)session.Instantiate(userId) ?? new AutomatedAgents(session).Guest;
            }
        }
    }
}
