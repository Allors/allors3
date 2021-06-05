// <copyright file="ToJsonVisitor.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Tests
{
    using System.Collections.Generic;
    using Database.Data;

    public class Arguments : IArguments
    {
        private readonly IDictionary<string, object> arguments;

        public Arguments(IDictionary<string, object> arguments) => this.arguments = arguments;

        public bool HasArgument(string name) => this.arguments.ContainsKey(name);

        public object ResolveUnit(int tag, string name) => this.arguments[name];

        public object[] ResolveUnits(int tag, string name) => (object[])this.arguments[name];

        public long ResolveObject(string name) => (long)this.arguments[name];

        public long[] ResolveObjects(string name) => (long[])this.arguments[name];

        public int ResolveMetaObject(string name) => (int)this.arguments[name];
    }
}