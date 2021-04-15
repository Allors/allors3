// <copyright file="OperandType.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Meta
{
    using System;

    public abstract class OperandType : MetaObjectBase, IOperandType
    {
        protected OperandType(MetaPopulation metaPopulation)
            : base(metaPopulation)
        {
        }

        public abstract string DisplayName { get; }

        public abstract Guid OperandId { get; }
    }
}
