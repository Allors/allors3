// <copyright file="ICompositeBase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Meta
{
    using System;

    public partial interface IObjectTypeBase : IMetaObjectBase, IObjectType
    {
        Type ClrType { get; }

        void Validate(ValidationLog log);
    }
}
