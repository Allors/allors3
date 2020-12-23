// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Meta;
    using Database.Derivations;

    public class CustomerShipmentStateDerivation : DomainDerivation
    {
        public CustomerShipmentStateDerivation(M m) : base(m, new Guid("7ff771c2-af69-427a-aeb0-4ceb73e699c7")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(m.CustomerShipment.ShipToParty),
                new ChangedPattern(m.CustomerShipment.ShipmentItems),
                new ChangedPattern(m.CustomerShipment.ShipmentState),
                new ChangedPattern(m.ShipmentItem.Quantity) { Steps =  new IPropertyType[] {m.ShipmentItem.ShipmentWhereShipmentItem}, OfType = m.CustomerShipment.Class },
                new ChangedPattern(m.PackagingContent.Quantity) { Steps =  new IPropertyType[] {m.PackagingContent.ShipmentItem, m.ShipmentItem.ShipmentWhereShipmentItem}, OfType = m.CustomerShipment.Class },
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<CustomerShipment>())
            {
                if (@this.ExistShipToParty && @this.ExistShipmentItems)
                {
                    //cancel shipment if nothing left to ship
                    if (@this.ExistShipmentItems && @this.PendingPickList == null
                        && !@this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Cancelled))
                    {
                        var canCancel = true;
                        foreach (ShipmentItem shipmentItem in @this.ShipmentItems)
                        {
                            if (shipmentItem.Quantity > 0)
                            {
                                canCancel = false;
                                break;
                            }
                        }

                        if (canCancel)
                        {
                            @this.Cancel();
                        }
                    }

                    if ((@this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Picking) ||
                         @this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Picked)) &&
                        @this.ShipToParty.ExistPickListsWhereShipToParty)
                    {
                        var isPicked = true;
                        foreach (PickList pickList in @this.ShipToParty.PickListsWhereShipToParty)
                        {
                            if (@this.Store.Equals(pickList.Store) &&
                                !pickList.PickListState.Equals(new PickListStates(@this.Session()).Picked))
                            {
                                isPicked = false;
                            }
                        }

                        if (isPicked)
                        {
                            @this.SetPicked();
                        }
                    }

                    if (@this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Picked)
                        || @this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Packed))
                    {
                        var totalShippingQuantity = 0M;
                        var totalPackagedQuantity = 0M;
                        foreach (ShipmentItem shipmentItem in @this.ShipmentItems)
                        {
                            totalShippingQuantity += shipmentItem.Quantity;
                            foreach (PackagingContent packagingContent in shipmentItem.PackagingContentsWhereShipmentItem)
                            {
                                totalPackagedQuantity += packagingContent.Quantity;
                            }
                        }

                        if (@this.Store.IsImmediatelyPacked && totalPackagedQuantity == totalShippingQuantity)
                        {
                            @this.SetPacked();
                        }
                    }

                    if (@this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Created)
                        || @this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Picked)
                        || @this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Picked)
                        || @this.ShipmentState.Equals(new ShipmentStates(@this.Session()).Packed))
                    {
                        if (@this.ShipmentValue < @this.Store.ShipmentThreshold && !@this.ReleasedManually)
                        {
                            @this.PutOnHold();
                        }
                    }

                    if (@this.ShipmentState.Equals(new ShipmentStates(@this.Session()).OnHold) &&
                        !@this.HeldManually &&
                        ((@this.ShipmentValue >= @this.Store.ShipmentThreshold) || @this.ReleasedManually))
                    {
                        @this.Continue();
                    }
                }
            }
        }
    }
}