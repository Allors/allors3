// <copyright file="PermissionsCache.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Domain;

    public class PermissionsCacheEntry: IPermissionsCacheEntry
    {
        public Guid ClassId { get; }

        public IReadOnlyDictionary<Guid, long> RoleReadPermissionIdByRelationTypeId { get; }

        public IReadOnlyDictionary<Guid, long> RoleWritePermissionIdByRelationTypeId { get; }

        public IReadOnlyDictionary<Guid, long> MethodExecutePermissionIdByMethodTypeId { get; }

        public PermissionsCacheEntry(IGrouping<Guid, Permission> permissionsByClassId)
        {
            this.ClassId = permissionsByClassId.Key;
            this.RoleReadPermissionIdByRelationTypeId = permissionsByClassId.OfType<ReadPermission>().ToDictionary(v => v.RelationTypePointer, v => v.Id);
            this.RoleWritePermissionIdByRelationTypeId = permissionsByClassId.OfType<WritePermission>().ToDictionary(v => v.RelationTypePointer, v => v.Id);
            this.MethodExecutePermissionIdByMethodTypeId = permissionsByClassId.OfType<ExecutePermission>().ToDictionary(v => v.MethodTypePointer, v => v.Id);
        }
    }
}
