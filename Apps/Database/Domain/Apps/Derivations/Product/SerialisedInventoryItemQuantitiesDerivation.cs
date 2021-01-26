// <copyright file="SerialisedInventoryItemQuantitiesDerivation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Database.Derivations;
    using Meta;

    public class SerialisedInventoryItemQuantitiesDerivation : DomainDerivation
    {
        public SerialisedInventoryItemQuantitiesDerivation(M m) : base(m, new Guid("0dd99432-c8e6-4278-8f49-fb1a4d7d6ddc")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(m.SerialisedInventoryItem.SerialisedInventoryItemState),
                new ChangedPattern(m.InventoryItemTransaction.InventoryItem) { Steps = new IPropertyType[] {m.InventoryItemTransaction.InventoryItem }, OfType = m.SerialisedInventoryItem.Class },
                new ChangedPattern(m.InventoryItemTransaction.Quantity) { Steps = new IPropertyType[] {m.InventoryItemTransaction.InventoryItem }, OfType = m.SerialisedInventoryItem.Class },
                new ChangedPattern(m.PickListItem.InventoryItem) { Steps = new IPropertyType[] {m.PickListItem.InventoryItem }, OfType = m.SerialisedInventoryItem.Class },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<SerialisedInventoryItem>())
            {
                var settings = @this.Strategy.Session.GetSingleton().Settings;

                @this.Quantity = 0;

                if (!settings.InventoryStrategy.OnHandSerialisedStates.Contains(@this.SerialisedInventoryItemState))
                {
                    @this.Quantity = 0;
                }
                else
                {
                    foreach (InventoryItemTransaction inventoryTransaction in @this.InventoryItemTransactionsWhereInventoryItem)
                    {
                        var reason = inventoryTransaction.Reason;

                        if (reason.IncreasesQuantityOnHand == true)
                        {
                            @this.Quantity += (int)inventoryTransaction.Quantity;
                        }
                        else if (reason.IncreasesQuantityOnHand == false)
                        {
                            @this.Quantity -= (int)inventoryTransaction.Quantity;
                        }
                    }
                }

                foreach (PickListItem pickListItem in @this.PickListItemsWhereInventoryItem)
                {
                    if (pickListItem.PickListWherePickListItem.PickListState.Equals(new PickListStates(@this.Strategy.Session).Picked))
                    {
                        foreach (ItemIssuance itemIssuance in pickListItem.ItemIssuancesWherePickListItem)
                        {
                            if (!itemIssuance.ShipmentItem.ShipmentItemState.Shipped)
                            {
                                @this.Quantity -= (int)pickListItem.QuantityPicked;
                            }
                        }
                    }
                }

                if (@this.Quantity < 0 || @this.Quantity > 1)
                {
                    var message = "Invalid transaction";
                    cycle.Validation.AddError($"{@this} {@this.Meta.Quantity} {message}");
                }
            }
        }
    }
}