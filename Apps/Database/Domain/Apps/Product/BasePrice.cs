// <copyright file="BasePrice.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;

namespace Allors.Database.Domain
{
    public partial class BasePrice
    {
        public void AppsDelete(DeletableDelete method)
        {
            this.Product.RemoveBasePrice(this);

            this.ProductFeature.RemoveFromBasePrices(this);
        }
    }
}
