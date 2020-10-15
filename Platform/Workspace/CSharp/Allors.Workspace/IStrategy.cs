// <copyright file="Object.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System.Collections.Generic;
    using Allors.Workspace.Meta;

    public interface IStrategy
    {
        IObject Object { get; }

        long WorkspaceId { get; }

        IClass Class { get; }

        ISession Session { get; }

        bool Exist(IRoleType roleType);

        object Get(IRoleType roleType);

        void Set(IRoleType roleType, object value);

        void Add(IRoleType roleType, IObject value);

        void Remove(IRoleType roleType, IObject value);

        IObject GetAssociation(IAssociationType associationType);

        IEnumerable<IObject> GetAssociations(IAssociationType associationType);
    }
}
