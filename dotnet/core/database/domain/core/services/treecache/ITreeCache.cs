// <copyright file="ITreeCache.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Allors.Database.Meta;
    using Database.Data;

    // TODO: Remove
    public partial interface ITreeCache
    {
        Node[] Get(IComposite composite);

        void Set(IComposite composite, Node[] tree);
    }
}
