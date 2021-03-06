// <copyright file="RecordBasedOriginState.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters
{
    using System.Collections.Generic;
    using Meta;
    using Ranges;

    public abstract class RecordBasedOriginState
    {
        public abstract Strategy Strategy { get; }

        protected bool HasChanges => this.Record == null || this.ChangedRoleByRelationType?.Count > 0;

        protected abstract IEnumerable<IRoleType> RoleTypes { get; }

        protected abstract IRecord Record { get; }

        protected IRecord PreviousRecord { get; set; }

        public Dictionary<IRelationType, object> ChangedRoleByRelationType { get; protected set; }

        private Dictionary<IRelationType, object> PreviousChangedRoleByRelationType { get; set; }

        public object GetUnitRole(IRoleType roleType)
        {
            if (this.ChangedRoleByRelationType != null && this.ChangedRoleByRelationType.TryGetValue(roleType.RelationType, out var role))
            {
                return role;
            }

            return this.Record?.GetRole(roleType);
        }

        public void SetUnitRole(IRoleType roleType, object role) => this.SetChangedRole(roleType, role);

        public long? GetCompositeRole(IRoleType roleType)
        {
            if (this.ChangedRoleByRelationType != null && this.ChangedRoleByRelationType.TryGetValue(roleType.RelationType, out var role))
            {
                return (long?)role;
            }

            return (long?)this.Record?.GetRole(roleType);
        }

        public void SetCompositeRole(IRoleType roleType, long? role)
        {
            var previousRole = this.GetCompositeRole(roleType);

            if (previousRole == role)
            {
                return;
            }

            var associationType = roleType.AssociationType;
            if (associationType.IsOne && role.HasValue)
            {
                var previousAssociation = this.Session.GetCompositeAssociation(role.Value, associationType);
                this.SetChangedRole(roleType, role);
                if (associationType.IsOne && previousAssociation != null)
                {
                    // OneToOne
                    previousAssociation.SetRole(roleType, null);
                }
            }
            else
            {
                this.SetChangedRole(roleType, role);
            }
        }

        public IRange GetCompositesRole(IRoleType roleType)
        {
            if (this.ChangedRoleByRelationType != null && this.ChangedRoleByRelationType.TryGetValue(roleType.RelationType, out var role))
            {
                return (IRange)role;
            }

            return this.Ranges.Ensure(this.Record?.GetRole(roleType));
        }

        public void AddCompositesRole(IRoleType roleType, long roleToAdd)
        {
            var associationType = roleType.AssociationType;

            var previousRole = this.GetCompositesRole(roleType);
            var previousAssociation = this.Session.GetCompositeAssociation(roleToAdd, associationType);

            if (previousRole.Contains(roleToAdd))
            {
                return;
            }

            var role = this.Ranges.Add(previousRole, roleToAdd);
            this.SetChangedRole(roleType, role);
            if (associationType.IsMany)
            {
                return;
            }

            // OneToMany
            //var previousAssociation = this.Session.GetCompositeAssociation(roleToAdd, associationType);
            previousAssociation?.SetRole(roleType, null);
        }

        public void RemoveCompositesRole(IRoleType roleType, long roleToRemove)
        {
            var previousRole = this.GetCompositesRole(roleType);

            if (!previousRole.Contains(roleToRemove))
            {
                return;
            }

            var role = this.Ranges.Remove(previousRole, roleToRemove);
            this.SetChangedRole(roleType, role);
        }

        public void SetCompositesRole(IRoleType roleType, IRange role)
        {
            var previousRole = this.GetCompositesRole(roleType);

            this.SetChangedRole(roleType, role);

            var associationType = roleType.AssociationType;
            if (associationType.IsMany)
            {
                return;
            }

            // OneToMany
            foreach (var addedRole in this.Ranges.Except(role, previousRole))
            {
                var previousAssociation = this.Session.GetCompositeAssociation(addedRole, associationType);
                previousAssociation?.SetRole(roleType, null);
            }
        }

        public void Checkpoint(ChangeSet changeSet)
        {
            // Same record
            if (this.PreviousRecord == null || this.Record == null || this.Record.Version == this.PreviousRecord.Version)
            {
                // No previous changed roles
                if (this.PreviousChangedRoleByRelationType == null)
                {
                    if (this.ChangedRoleByRelationType != null)
                    {
                        // Changed roles
                        foreach (var kvp in this.ChangedRoleByRelationType)
                        {
                            var current = kvp.Value;
                            var relationType = kvp.Key;
                            var previous = this.Record?.GetRole(relationType.RoleType);
                            changeSet.Diff(this.Strategy, relationType, current, previous);
                        }
                    }
                }
                // Previous changed roles
                else
                {
                    foreach (var kvp in this.ChangedRoleByRelationType)
                    {
                        var relationType = kvp.Key;
                        var current = kvp.Value;

                        this.PreviousChangedRoleByRelationType.TryGetValue(relationType, out var previous);
                        changeSet.Diff(this.Strategy, relationType, current, previous);
                    }
                }
            }
            // Different record
            else
            {
                foreach (var roleType in this.RoleTypes)
                {
                    var relationType = roleType.RelationType;

                    object previous = null;
                    object current = null;

                    if (this.PreviousChangedRoleByRelationType?.TryGetValue(relationType, out previous) == true)
                    {
                        if (this.ChangedRoleByRelationType?.TryGetValue(relationType, out current) == true)
                        {
                            changeSet.Diff(this.Strategy, relationType, current, previous);
                        }
                        else
                        {
                            current = this.Record.GetRole(roleType);
                            changeSet.Diff(this.Strategy, relationType, current, previous);
                        }
                    }
                    else
                    {
                        previous = this.PreviousRecord?.GetRole(roleType);
                        if (this.ChangedRoleByRelationType?.TryGetValue(relationType, out current) == true)
                        {
                            changeSet.Diff(this.Strategy, relationType, current, previous);
                        }
                        else
                        {
                            current = this.Record.GetRole(roleType);
                            changeSet.Diff(this.Strategy, relationType, current, previous);
                        }
                    }
                }
            }

            this.PreviousRecord = this.Record;
            this.PreviousChangedRoleByRelationType = this.ChangedRoleByRelationType;
        }

        public bool IsAssociationForRole(IRoleType roleType, long forRole)
        {
            if (roleType.IsOne)
            {
                var compositeRole = this.GetCompositeRole(roleType);
                return compositeRole == forRole;
            }

            var compositesRole = this.GetCompositesRole(roleType);
            return compositesRole.Contains(forRole);
        }

        protected abstract void OnChange();

        private void SetChangedRole(IRoleType roleType, object role)
        {
            this.ChangedRoleByRelationType ??= new Dictionary<IRelationType, object>();
            this.ChangedRoleByRelationType[roleType.RelationType] = role;
            this.OnChange();
        }

        #region Proxy Properties

        protected long Id => this.Strategy.Id;

        protected IClass Class => this.Strategy.Class;

        protected Session Session => this.Strategy.Session;

        protected Workspace Workspace => this.Session.Workspace;

        private IRanges Ranges => this.Strategy.Session.Workspace.Ranges;

        #endregion
    }
}
