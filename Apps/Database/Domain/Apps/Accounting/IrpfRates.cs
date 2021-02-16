// <copyright file="IrpfRates.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;

    public partial class IrpfRates
    {
        public static readonly Guid ZeroId = new Guid("438a01c9-e6b9-4aaf-88f5-7654cc8bc524");
        public static readonly Guid fifteenId = new Guid("46eb2dbc-98b2-4a23-9b1b-17edbccda748");
        public static readonly Guid NineteenId = new Guid("8d7ba239-caf8-41e8-a6b8-cbf10fc7223f");

        private UniquelyIdentifiableCache<IrpfRate> cache;

        public IrpfRate Zero => this.Cache[ZeroId];

        public IrpfRate fifteen => this.Cache[fifteenId];

        public IrpfRate nineteen => this.Cache[NineteenId];

        private UniquelyIdentifiableCache<IrpfRate> Cache => this.cache ??= new UniquelyIdentifiableCache<IrpfRate>(this.Transaction);

        protected override void AppsSetup(Setup setup)
        {
            var merge = this.Cache.Merger().Action();

            merge(ZeroId, v => v.Rate = 0);
            merge(fifteenId, v => v.Rate = 15);
            merge(NineteenId, v => v.Rate = 19);
        }
    }
}
