// <copyright file="WorkspaceOriginState.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters
{
    using System.Collections.Generic;
    using Meta;

    public sealed class WorkspaceOriginState : RecordBasedOriginState
    {
        public WorkspaceOriginState(Strategy strategy, WorkspaceRecord record)
        {
            this.Strategy = strategy;
            this.WorkspaceRecord = record;
            this.PreviousRecord = this.WorkspaceRecord;
        }

        public override Strategy Strategy { get; }

        protected override IEnumerable<IRoleType> RoleTypes => this.Class.WorkspaceOriginRoleTypes;

        protected override IRecord Record => this.WorkspaceRecord;

        private WorkspaceRecord WorkspaceRecord { get; set; }

        public long Version => this.WorkspaceRecord.Version;

        protected override void OnChange()
        {
            this.Strategy.Session.ChangeSetTracker.OnWorkspaceChanged(this);
            this.Strategy.Session.PushToWorkspaceTracker.OnChanged(this);
        }

        public void Push()
        {
            if (this.HasChanges)
            {
                this.Workspace.Push(this.Id, this.Class, this.Record?.Version ?? 0, this.ChangedRoleByRelationType);
            }

            this.WorkspaceRecord = this.Workspace.GetRecord(this.Id);
            this.ChangedRoleByRelationType = null;
        }

        public void Pull()
        {
            // TODO: Check for overwrites
            this.WorkspaceRecord = this.Workspace.GetRecord(this.Id);
        }
    }
}
