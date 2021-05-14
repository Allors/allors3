// <copyright file="WorkspaceOriginState.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters.Local
{
    using System.Collections.Generic;
    using Meta;

    internal sealed class WorkspaceOriginState : RecordBasedOriginState
    {
        internal WorkspaceOriginState(Strategy strategy, WorkspaceRecord record) : base(strategy)
        {
            this.WorkspaceRecord = record;
            this.PreviousRecord = this.WorkspaceRecord;
        }

        protected override IEnumerable<IRoleType> RoleTypes => this.Class.WorkspaceOriginRoleTypes;

        protected override IRecord Record => this.WorkspaceRecord;

        private WorkspaceRecord WorkspaceRecord { get; set; }

        protected override void OnChange()
        {
            this.Strategy.Session.ChangeSetTracker.OnChanged(this);
            this.Strategy.Session.PushToWorkspaceTracker.OnChanged(this);
        }

        internal void Push()
        {
            if (this.HasChanges)
            {
                this.Workspace.Push(this.Id, this.Class, this.Record?.Version ?? 0, this.ChangedRoleByRelationType);
            }

            this.WorkspaceRecord = this.Workspace.GetRecord(this.Id);
            this.ChangedRoleByRelationType = null;
        }
    }
}
