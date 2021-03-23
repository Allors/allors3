// <copyright file="IWorkspaceLifecycle.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System;

    public interface IWorkspaceLifecycle : IDisposable
    {
        void OnInit(IWorkspace internalWorkspace);

        ISessionLifecycle CreateSessionContext();
    }
}
