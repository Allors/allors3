// <copyright file="DatabaseObject.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;

    internal static class RoleTypeExtensions
    {
        internal static object NormalizeUnit(this IRoleType @this, object role)
        {
            if (role is DateTime dateTime && dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
            {
                switch (dateTime.Kind)
                {
                    case DateTimeKind.Local:
                        dateTime = dateTime.ToUniversalTime();
                        break;
                    case DateTimeKind.Unspecified:
                        throw new ArgumentException(
                            @"DateTime value is of DateTimeKind.Kind Unspecified.
Unspecified is only allowed for DateTime.MaxValue and DateTime.MinValue. 
Use DateTimeKind.Utc or DateTimeKind.Local.");
                }

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Utc);
            }

            var clrType = @this.ObjectType.ClrType;
            if (role.GetType() != clrType)
            {
                role = Convert.ChangeType(role, clrType);
            }

            return role;
        }

        internal static long NormalizeComposite(this IRoleType @this, object role) =>
            role switch
            {
                long @long => @long,
                IObject @object => @object.WorkspaceId,
                _ => throw new ArgumentException("Illegal composite role: ${role}")
            };

        internal static long[] NormalizeComposites(this IRoleType @this, object role) =>
            role switch
            {
                long[] longs => longs,
                IEnumerable<IObject> objects => objects.Select(v => v.WorkspaceId).ToArray(),
                _ => throw new ArgumentException("Illegal composites role: ${role}")
            };
    }
}