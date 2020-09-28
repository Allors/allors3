// <copyright file="IPattern.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDomainDerivation type.</summary>

namespace Allors
{
    using Meta;

    public abstract class Pattern
    {
        public IPropertyType[] Steps { get; set; }

        public IComposite OfType { get; set; }
    }
}
