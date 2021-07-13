// <copyright file="Workspace.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters
{
    using System.Collections.Generic;

    public class PushResult : IPushResult
    {
        public bool HasErrors { get; }
        public string ErrorMessage { get; }
        public IEnumerable<IObject> VersionErrors { get; }
        public IEnumerable<IObject> AccessErrors { get; }
        public IEnumerable<IObject> MissingErrors { get; }
        public IEnumerable<IDerivationError> DerivationErrors { get; }
    }
}
