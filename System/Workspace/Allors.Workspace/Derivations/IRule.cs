// <copyright file="IRule.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDerivation type.</summary>

namespace Allors.Workspace.Derivations
{
    using System;

    public interface IRule
    {
        Guid Id { get; }

        Pattern[] Patterns { get; }

        void Match(ICycle cycle, IObject match);
    }
}
