// <copyright file="TreeNode.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Data
{
    using System.Collections.Generic;
    using System.Linq;

    public static class TreeNodeExtensions
    {
        public static PrefetchPolicy BuildPrefetchPolicy(this TreeNode[] treeNodes)
        {
            var prefetchPolicyBuilder = new PrefetchPolicyBuilder();

            foreach (var node in treeNodes)
            {
                node.BuildPrefetchPolicy(prefetchPolicyBuilder);
            }

            return prefetchPolicyBuilder.Build();
        }

        public static void Resolve(this TreeNode[] treeNodes, IObject obj, HashSet<IObject> objects)
        {
            if (obj != null)
            {
                foreach (var node in treeNodes)
                {
                    node.Resolve(obj, objects);
                }
            }
        }

        public static Protocol.Data.TreeNode[] Save(this TreeNode[] treeNodes) =>
            treeNodes.Select(v => v.Save()).ToArray();
    }
}