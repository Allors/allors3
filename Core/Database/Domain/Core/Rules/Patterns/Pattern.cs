// <copyright file="IPattern.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDomainDerivation type.</summary>

namespace Allors.Database.Derivations
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Meta;

    public abstract class Pattern : IPattern
    {
        // TODO: Remove
        public IPropertyType[] Steps
        {
            private get => throw new NotSupportedException();

            set
            {
                var node = value.ToPath();
                this.Tree = value != null ? new Node[] { node } : Array.Empty<Node>();
            }
        }

        IEnumerable<Node> IPattern.Tree => this.Tree;
        public IEnumerable<Node> Tree { get; set; }

        public IComposite OfType { get; set; }

        public abstract IComposite ObjectType { get; }
    }
}
