// <copyright file="IStickyService.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.State
{
    using System.Collections.Generic;

    public interface IStickyService
    {
        IDictionary<T, long> Get<T>(string key);

        void Set<T>(string key, IDictionary<T, long> value);
    }
}
