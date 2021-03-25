// <copyright file="IProcedureInput.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database
{
    using System.Collections.Generic;

    public interface IProcedureInput
    {
        IDictionary<string, IObject[]> Collections { get; }

        IDictionary<string, IObject> Objects { get; }

        IDictionary<string, object> Values { get; }

        public T[] GetCollection<T>();

        public T[] GetCollection<T>(string key);

        public T GetObject<T>() where T : class, IObject;

        public T GetObject<T>(string key) where T : class, IObject;

        public object GetValue(string key);

        public T GetValue<T>(string key);
    }
}
