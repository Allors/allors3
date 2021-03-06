// <copyright file="SessionContext.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using Domain;

    public partial class SessionContext : ISessionContext
    {
        public void Dispose()
        {
        }

        public void OnInit(ISession internalSession)
        {
        }
    }
}
