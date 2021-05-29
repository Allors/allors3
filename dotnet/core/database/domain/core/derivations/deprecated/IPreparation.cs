// <copyright file="IValidation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Derivations.Compat
{
    using System.Collections.Generic;

    public interface IPreparation
    {
        ISet<Object> Objects { get; }
    }
}
