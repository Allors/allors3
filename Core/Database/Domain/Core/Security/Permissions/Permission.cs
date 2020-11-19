// <copyright file="Permission.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Meta;
    using Database.Security;

    public partial interface Permission : IPermission
    {
        bool ExistConcreteClass { get; }

        bool ExistOperandType { get; }

        bool ExistOperation { get; }

        OperandType OperandType { get; }

        Operations Operation { get; }

        bool InWorkspace(string workspaceName);
    }
}
