// <copyright file="SerialisedInventoryItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class SerialisedInventoryItem
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.SerialisedInventoryItem, this.M.SerialisedInventoryItem.SerialisedInventoryItemState),
        };

        public InventoryStrategy InventoryStrategy
            => this.Strategy.Session.GetSingleton().Settings.InventoryStrategy;

        public int QuantityOnHand
            => this.InventoryStrategy.OnHandSerialisedStates.Contains(this.SerialisedInventoryItemState) ? this.Quantity : 0;

        public int AvailableToPromise
            => this.InventoryStrategy.AvailableToPromiseSerialisedStates.Contains(this.SerialisedInventoryItemState) ? this.Quantity : 0;

        public void AppsDelete(DeletableDelete method)
        {
            foreach (InventoryItemVersion version in this.AllVersions)
            {
                version.Delete();
            }
        }
    }
}
