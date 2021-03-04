// <copyright file="Object.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;

    internal sealed class RemoteWorkspaceState
    {
        private readonly RemoteStrategy strategy;

        private Dictionary<IRelationType, object> changedRoleByRoleType;

        private RemoteWorkspaceObject workspaceObject;

        private RemoteWorkspaceObject changeSetWorkspaceObject;

        internal RemoteWorkspaceState(RemoteStrategy strategy)
        {
            this.strategy = strategy;
            this.workspaceObject = this.Workspace.Get(this.Identity);
        }

        internal bool HasWorkspaceChanges => this.changedRoleByRoleType != null;

        private Identity Identity => this.strategy.Identity;

        private IClass Class => this.strategy.Class;

        private RemoteSession Session => this.strategy.Session;

        private RemoteWorkspace Workspace => this.Session.Workspace;

        internal object GetRole(IRoleType roleType)
        {
            if (roleType.ObjectType.IsUnit)
            {

                if (this.changedRoleByRoleType == null || !this.changedRoleByRoleType.TryGetValue(roleType.RelationType, out var unit))
                {
                    unit = this.workspaceObject?.GetRole(roleType);
                }

                return unit;
            }

            if (roleType.IsOne)
            {
                if (this.changedRoleByRoleType == null || !this.changedRoleByRoleType.TryGetValue(roleType.RelationType, out var workspaceRole))
                {
                    workspaceRole = (Identity)this.workspaceObject?.GetRole(roleType);
                }

                return this.Session.Instantiate<IObject>((Identity)workspaceRole);
            }

            if (this.changedRoleByRoleType == null || !this.changedRoleByRoleType.TryGetValue(roleType.RelationType, out var identities))
            {
                identities = (Identity[])this.workspaceObject?.GetRole(roleType);
            }

            var ids = (Identity[])identities;

            if (ids == null)
            {
                return this.Session.Workspace.ObjectFactory.EmptyArray(roleType.ObjectType);
            }

            var array = Array.CreateInstance(roleType.ObjectType.ClrType, ids.Length);
            for (var i = 0; i < ids.Length; i++)
            {
                array.SetValue(this.Session.Instantiate<IObject>(ids[i]), i);
            }

            return array;
        }

        internal void SetRole(IRoleType roleType, object value)
        {
            var current = this.GetRole(roleType);
            if (roleType.ObjectType.IsUnit || roleType.IsOne)
            {
                if (Equals(current, value))
                {
                    return;
                }
            }
            else
            {
                value ??= Array.Empty<IStrategy>();

                var currentCollection = (IList<object>)current;
                var valueCollection = (IList<object>)value;
                if (currentCollection.Count == valueCollection.Count &&
                    !currentCollection.Except(valueCollection).Any())
                {
                    return;
                }
            }

            this.changedRoleByRoleType ??= new Dictionary<IRelationType, object>();

            if (roleType.ObjectType.IsUnit)
            {
                this.changedRoleByRoleType[roleType.RelationType] = value;
            }
            else
            {
                if (roleType.IsOne)
                {
                    this.changedRoleByRoleType[roleType.RelationType] = ((IObject)value)?.Identity;
                }
                else
                {
                    this.changedRoleByRoleType[roleType.RelationType] = ((IEnumerable<object>)value).Select(v => ((IObject)v).Identity).ToArray();
                }
            }

            this.Session.OnChange(this);
        }

        internal void Push()
        {
            if (this.HasWorkspaceChanges)
            {
                this.Workspace.Push(this.Identity, this.Class, this.workspaceObject?.Version ?? 0, this.changedRoleByRoleType);
            }

            this.Reset();
        }

        internal void Reset()
        {
            this.workspaceObject = this.Workspace.Get(this.Identity);
            this.changedRoleByRoleType = null;
        }
    }
}
