// <copyright file="ChangedRoles.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDomainDerivation type.</summary>

namespace Allors.Database.Data
{
    using Allors.Database.Meta;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static partial class ICompositeExtensions
    {
        public static IEnumerable<Node> Nodes<T>(this T @this, params Func<T, Node>[] children) where T : IComposite => children.Select(v => v(@this));

        public static IEnumerable<Node> Nodes<T>(this T @this, Func<T, IEnumerable<Node>> children) where T : IComposite => children(@this);

        public static IEnumerable<Node> Nodes<T>(this T @this, params Expression<Func<T, IPropertyType>>[] children) where T : IComposite => children.Select(v => v.Node(@this.MetaPopulation));

        public static IEnumerable<Node> Nodes<T>(this T @this, params Expression<Func<T, IComposite>>[] children) where T : IComposite => children.Select(v => v.Node(@this.MetaPopulation));

        public static Node Node<T>(this T @this, Func<T, Node> child) where T : IComposite => child(@this);

        public static Node Node<T>(this T @this, Expression<Func<T, IPropertyType>> path) where T : IComposite => path.Node(@this.MetaPopulation);

        public static Node Node<T>(this T @this, Expression<Func<T, IComposite>> path) where T : IComposite => path.Node(@this.MetaPopulation);
    }
}
