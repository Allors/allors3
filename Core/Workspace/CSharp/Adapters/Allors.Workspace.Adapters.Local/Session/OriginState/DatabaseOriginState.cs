// <copyright file="Object.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters.Local
{
    using Meta;

    internal sealed class DatabaseOriginState : RecordBasedOriginState
    {
        internal DatabaseOriginState(Strategy strategy, DatabaseRecord record) : base(strategy, record) { }

        public bool CanRead(IRoleType roleType)
        {
            if (!this.ExistDatabaseObjects)
            {
                return true;
            }

            var permission = this.Session.Workspace.DatabaseAdapter.GetPermission(this.Class, roleType, Operations.Read);
            return this.DatabaseRecord.IsPermitted(permission);
        }

        public bool CanWrite(IRoleType roleType)
        {
            if (!this.ExistDatabaseObjects)
            {
                return true;
            }

            var permission = this.Session.Workspace.DatabaseAdapter.GetPermission(this.Class, roleType, Operations.Write);
            return this.DatabaseRecord.IsPermitted(permission);
        }

        public bool CanExecute(IMethodType methodType)
        {
            if (!this.ExistDatabaseObjects)
            {
                return true;
            }

            var permission = this.Session.Workspace.DatabaseAdapter.GetPermission(this.Class, methodType, Operations.Execute);
            return this.DatabaseRecord.IsPermitted(permission);
        }

        private bool ExistDatabaseObjects => this.Record != null;

        private DatabaseRecord DatabaseRecord => (DatabaseRecord)this.Record;

        public long Version => this.DatabaseRecord.Version;

        public override bool HasChanges => this.Record == null || this.ChangedRoleByRelationType?.Count > 0;

        internal override void Reset()
        {
            this.Record = this.Session.DatabaseAdapter.Get(this.Id);
            this.ChangedRoleByRelationType = null;
        }

        protected override void OnChange() => this.Session.OnChange(this);

        internal void PushResponse(DatabaseRecord newDatabaseRecord) => this.Record = newDatabaseRecord;
    }
}
