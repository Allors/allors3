// <copyright file="IBarcodeGenerator.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.State
{
    using System.Collections.Generic;
    using Meta;

    public interface IWorkspaceMetaCacheEntry
    {
        ISet<Class> Classes { get; }
    }
}
