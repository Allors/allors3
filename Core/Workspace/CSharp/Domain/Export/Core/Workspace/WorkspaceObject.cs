﻿// <copyright file="WorkspaceObject.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using Allors.Workspace.Meta;
    using System.Collections.Generic;
    using Allors.Protocol.Remote.Sync;

    public class WorkspaceObject
    {
        public WorkspaceObject(Workspace workspace, SyncResponse loadResponse, SyncResponseObject loadObject)
        {
            this.Workspace = workspace;
            this.Id = long.Parse(loadObject.I);
            this.Version = !string.IsNullOrEmpty(loadObject.V) ? long.Parse(loadObject.V) : 0;
            this.UserSecurityHash = loadResponse.UserSecurityHash;
            this.ObjectType = (Class)this.Workspace.ObjectFactory.GetObjectTypeForTypeName(loadObject.T);

            this.Roles = new Dictionary<string, object>();
            this.Methods = new Dictionary<string, object>();

            if (loadObject.Roles != null)
            {
                foreach (var role in loadObject.Roles)
                {
                    var name = (string)role[0];
                    var access = (string)role[1];
                    var canRead = access.Contains("r");
                    var canWrite = access.Contains("w");

                    this.Roles[$"CanRead{name}"] = canRead;
                    this.Roles[$"CanWrite{name}"] = canWrite;

                    if (canRead)
                    {
                        var value = role.Length > 2 ? role[2] : null;
                        this.Roles[name] = value;
                    }
                }
            }

            if (loadObject.Methods != null)
            {
                foreach (var method in loadObject.Methods)
                {
                    var name = method[0];
                    var access = method[1];
                    var canExecute = access.Contains("x");

                    this.Methods[$"CanExecute{name}"] = canExecute;
                }
            }
        }

        public Workspace Workspace { get; }

        public long Id { get; }

        public long Version { get; }

        public string UserSecurityHash { get; }

        public Class ObjectType { get; }

        public Dictionary<string, object> Roles { get; }

        public Dictionary<string, object> Methods { get; }

        public bool CanRead(string roleTypeName) => (bool)this.Roles[$"CanRead{roleTypeName}"];

        public bool CanWrite(string roleTypeName) => (bool)this.Roles[$"CanWrite{roleTypeName}"];

        public bool CanExecute(string methodTypeName) => (bool)this.Methods[$"CanExecute{methodTypeName}"];
    }
}
