// <copyright file="IPredicate.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Data
{
    using System.Collections.Generic;
    public interface IPredicate : IVisitable
    {
        string[] Dependencies { get; }

        void Build(ISession session, IDictionary<string, string> parameters, Allors.ICompositePredicate compositePredicate);

        bool ShouldTreeShake(IDictionary<string, string> parameters);

        bool HasMissingArguments(IDictionary<string, string> parameters);
    }
}
