// <copyright file="AutomatedAgent.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class AutomatedAgents
    {
        protected override void BasePrepare(Setup setup) => setup.AddDependency(this.ObjectType, this.M.AccessControl);
    }
}
