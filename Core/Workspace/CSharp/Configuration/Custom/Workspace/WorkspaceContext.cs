// <copyright file="IDatabaseScope.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System.Security;
    using Configuration;
    using Derivations;
    using Derivations.Default;
    using Domain;
    using Meta;

    public partial class WorkspaceContext : IWorkspaceContext
    {
        public M M { get; private set; }

        public IDerivationFactory DerivationFactory { get; private set; }

        public ITime Time { get; private set; }

        public void OnInit(IWorkspace workspace)
        {
            var metaPopulation = (MetaPopulation)workspace.MetaPopulation;

            this.M = new M(metaPopulation);

            var engine = new Engine(metaPopulation, this.CreateRules());
            this.DerivationFactory = new DerivationFactory(engine);

            this.Time = new Time();
        }

        public void Dispose()
        {
        }

        public ISessionLifecycle CreateSessionContext() => new SessionContext();

        private Rule[] CreateRules()
        {
            var m = this.M;

            return new Rule[]
            {
                new PersonSessionFullNameDerivation(m)
            };
        }
    }
}