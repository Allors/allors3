// <copyright file="PermissionsCache.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.State
{
    using System;
    using System.Collections.Generic;

    public interface IPermissionsCacheEntry
    {
        IReadOnlyDictionary<Guid, long> RoleReadPermissionIdByRelationTypeId { get; }

        IReadOnlyDictionary<Guid, long> RoleWritePermissionIdByRelationTypeId { get; }

        IReadOnlyDictionary<Guid, long> MethodExecutePermissionIdByMethodTypeId { get; }
    }
}
