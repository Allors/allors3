// <copyright file="IWorkspaceContext.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Domain
{
    using Meta;

    public partial interface IWorkspaceContext : IWorkspaceServices
    {
        M M { get; }

        IDerivationFactory DerivationFactory { get; }

        ITime Time { get; }
    }
}
