// <copyright file="SecurityResponseBuilder.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Protocol.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Protocol.Json.Api.Security;
    using Meta;

    public class SecurityResponseBuilder
    {
        private readonly ISession session;
        private readonly ISet<IClass> allowedClasses;

        public SecurityResponseBuilder(ISession session, IAccessControlLists accessControlLists, ISet<IClass> allowedClasses)
        {
            this.session = session;
            this.allowedClasses = allowedClasses;
            this.AccessControlLists = accessControlLists;
        }

        public IAccessControlLists AccessControlLists { get; }

        public SecurityResponse Build(SecurityRequest securityRequest)
        {
            var securityResponse = new SecurityResponse();

            if (securityRequest.AccessControls?.Length > 0)
            {
                var accessControlIds = securityRequest.AccessControls;
                var accessControls = this.session.Instantiate(accessControlIds).Cast<IAccessControl>().ToArray();

                securityResponse.AccessControls = accessControls
                    .Select(v =>
                    {
                        var response = new SecurityResponseAccessControl
                        {
                            Id = v.Strategy.ObjectId.ToString(),
                            Version = v.Strategy.ObjectVersion.ToString(),
                        };

                        if (this.AccessControlLists.EffectivePermissionIdsByAccessControl.TryGetValue(v, out var x))
                        {
                            response.PermissionIds = string.Join(",", x);
                        }

                        return response;
                    }).ToArray();
            }

            if (securityRequest.Permissions?.Length > 0)
            {
                var permissionIds = securityRequest.Permissions;
                var permissions = this.session.Instantiate(permissionIds)
                    .Cast<IPermission>()
                    .Where(v => this.allowedClasses?.Contains(v.ConcreteClass) == true);

                securityResponse.Permissions = permissions.Select(v =>
                    v switch
                    {
                        IReadPermission permission => new[]
                        {
                            permission.Strategy.ObjectId.ToString(),
                            permission.ConcreteClass.Id.ToString("D"),
                            permission.RelationType.Id.ToString("D"),
                            ((int)Operations.Read).ToString(),
                        },
                        IWritePermission permission => new[]
                        {
                            permission.Strategy.ObjectId.ToString(),
                            permission.ConcreteClass.Id.ToString("D"),
                            permission.RelationType.Id.ToString("D"),
                            ((int)Operations.Write).ToString(),
                        },
                        IExecutePermission permission => new[]
                        {
                            permission.Strategy.ObjectId.ToString(),
                            permission.ConcreteClass.Id.ToString("D"),
                            permission.MethodType.Id.ToString("D"),
                            ((int)Operations.Execute).ToString(),
                        },
                        _ => throw new Exception(),
                    }).ToArray();
            }

            return securityResponse;
        }
    }
}