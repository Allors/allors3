// <copyright file="IWorkspace.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System.Collections.Generic;
    using Derivations;
    using Meta;

    public interface IWorkspace
    {
        string Name { get; }

        IMetaPopulation MetaPopulation { get; }

        IObjectFactory ObjectFactory { get; }

        IWorkspaceLifecycle StateLifecycle { get; }

        IEnumerable<IDerivation> Derivations { get; }

        void AddDerivation(IDerivation derivation);

        ISession CreateSession();
    }
}
