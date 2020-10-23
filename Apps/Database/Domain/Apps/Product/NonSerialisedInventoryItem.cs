
// <copyright file="NonSerialisedInventoryItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class NonSerialisedInventoryItem
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.NonSerialisedInventoryItem, this.M.NonSerialisedInventoryItem.NonSerialisedInventoryItemState),
        };

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistNonSerialisedInventoryItemState)
            {
                this.NonSerialisedInventoryItemState = new NonSerialisedInventoryItemStates(this.Strategy.Session).Good;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            //var (iteration, changeSet, derivedObjects) = method;

            //if (iteration.IsMarked(this) || changeSet.IsCreated(this) || changeSet.HasChangedRoles(this))
            //{
            //    iteration.AddDependency(this.Part, this);
            //    iteration.Mark(this.Part);
            //}
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            //var derivation = method.Derivation;
            //var settings = this.Strategy.Session.GetSingleton().Settings;

            //if (!this.ExistName)
            //{
            //    this.Name = $"{this.Part?.Name} at {this.Facility?.Name} with state {this.NonSerialisedInventoryItemState?.Name}";
            //}

            //this.AppsOnDeriveUnitOfMeasure(derivation);

            //// QuantityOnHand
            //var quantityOnHand = 0M;

            //if (!settings.InventoryStrategy.OnHandNonSerialisedStates.Contains(this.NonSerialisedInventoryItemState))
            //{
            //    this.QuantityOnHand = 0;
            //}

            //foreach (InventoryItemTransaction inventoryTransaction in this.InventoryItemTransactionsWhereInventoryItem)
            //{
            //    var reason = inventoryTransaction.Reason;

            //    if (reason.IncreasesQuantityOnHand == true)
            //    {
            //        quantityOnHand += inventoryTransaction.Quantity;
            //    }
            //    else if (reason.IncreasesQuantityOnHand == false)
            //    {
            //        quantityOnHand -= inventoryTransaction.Quantity;
            //    }
            //}

            //foreach (PickListItem pickListItem in this.PickListItemsWhereInventoryItem)
            //{
            //    if (pickListItem.PickListWherePickListItem.PickListState.Equals(new PickListStates(this.Strategy.Session).Picked))
            //    {
            //        foreach (ItemIssuance itemIssuance in pickListItem.ItemIssuancesWherePickListItem)
            //        {
            //            if (!itemIssuance.ShipmentItem.ShipmentItemState.Shipped)
            //            {
            //                quantityOnHand -= pickListItem.QuantityPicked;
            //            }
            //        }
            //    }
            //}

            //this.QuantityOnHand = quantityOnHand;

            //// quantityCommittedOut
            //var quantityCommittedOut = 0M;

            //foreach (InventoryItemTransaction inventoryTransaction in this.InventoryItemTransactionsWhereInventoryItem)
            //{
            //    var reason = inventoryTransaction.Reason;

            //    if (reason.IncreasesQuantityCommittedOut == true)
            //    {
            //        quantityCommittedOut += inventoryTransaction.Quantity;
            //    }
            //    else if (reason.IncreasesQuantityCommittedOut == false)
            //    {
            //        quantityCommittedOut -= inventoryTransaction.Quantity;
            //    }
            //}

            //foreach (PickListItem pickListItem in this.PickListItemsWhereInventoryItem)
            //{
            //    if (pickListItem.PickListWherePickListItem.PickListState.Equals(new PickListStates(this.Strategy.Session).Picked))
            //    {
            //        foreach (ItemIssuance itemIssuance in pickListItem.ItemIssuancesWherePickListItem)
            //        {
            //            if (!itemIssuance.ShipmentItem.ShipmentItemState.Shipped)
            //            {
            //                quantityCommittedOut -= pickListItem.QuantityPicked;
            //            }
            //        }
            //    }
            //}

            //if (quantityCommittedOut < 0)
            //{
            //    quantityCommittedOut = 0;
            //}

            //this.QuantityCommittedOut = quantityCommittedOut;

            //// AvailableToPromise
            //var availableToPromise = this.QuantityOnHand - this.QuantityCommittedOut;

            //if (availableToPromise < 0)
            //{
            //    availableToPromise = 0;
            //}

            //this.AvailableToPromise = availableToPromise;

            //// QuantityExpectedIn
            //var quantityExpectedIn = 0M;

            //foreach (PurchaseOrderItem purchaseOrderItem in this.Part.PurchaseOrderItemsWherePart)
            //{
            //    var facility = purchaseOrderItem.PurchaseOrderWherePurchaseOrderItem.StoredInFacility;
            //    if ((purchaseOrderItem.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).InProcess)
            //         || purchaseOrderItem.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Sent))
            //        && this.Facility.Equals(facility))
            //    {
            //        quantityExpectedIn += purchaseOrderItem.QuantityOrdered;
            //        quantityExpectedIn -= purchaseOrderItem.QuantityReceived;
            //    }
            //}

            //this.QuantityExpectedIn = quantityExpectedIn;

            //// TODO: Remove OnDerive
            //this.Part.OnDerive(x => x.WithDerivation(derivation));
        }

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            //var derivation = method.Derivation;

            //if (this.ExistPreviousQuantityOnHand && this.QuantityOnHand > this.PreviousQuantityOnHand)
            //{
            //    this.AppsReplenishSalesOrders(derivation);
            //}

            //if (this.ExistPreviousQuantityOnHand && this.QuantityOnHand < this.PreviousQuantityOnHand)
            //{
            //    this.AppsDepleteSalesOrders(derivation);
            //}

            //this.PreviousQuantityOnHand = this.QuantityOnHand;
        }

        //public void AppsReplenishSalesOrders(IDerivation derivation)
        //{
        //    var salesOrderItems = this.Strategy.Session.Extent<SalesOrderItem>();
        //    salesOrderItems.Filter.AddEquals(M.SalesOrderItem.SalesOrderItemState, new SalesOrderItemStates(this.Strategy.Session).InProcess);
        //    salesOrderItems.AddSort(M.OrderItem.DeliveryDate, SortDirection.Ascending);
        //    var nonUnifiedGoods = this.Part.NonUnifiedGoodsWherePart;
        //    var unifiedGood = this.Part as UnifiedGood;

        //    if (nonUnifiedGoods.Count > 0 || unifiedGood != null)
        //    {
        //        if (unifiedGood != null)
        //        {
        //            salesOrderItems.Filter.AddEquals(M.SalesOrderItem.Product, unifiedGood);
        //        }

        //        if (nonUnifiedGoods.Count > 0)
        //        {
        //            salesOrderItems.Filter.AddContainedIn(M.SalesOrderItem.Product, (Extent)nonUnifiedGoods);
        //        }

        //        salesOrderItems = this.Strategy.Session.Instantiate(salesOrderItems);

        //        var extra = this.QuantityOnHand - this.PreviousQuantityOnHand;

        //        foreach (SalesOrderItem salesOrderItem in salesOrderItems)
        //        {
        //            if (extra > 0 && salesOrderItem.QuantityShortFalled > 0)
        //            {
        //                decimal diff;
        //                if (extra >= salesOrderItem.QuantityShortFalled)
        //                {
        //                    diff = salesOrderItem.QuantityShortFalled;
        //                }
        //                else
        //                {
        //                    diff = extra;
        //                }

        //                extra -= diff;

        //                // HACK: DerivedRoles
        //                var salesOrderItemDerivedRoles = salesOrderItem;

        //                salesOrderItemDerivedRoles.QuantityShortFalled -= diff;

        //                // var inventoryAssignment = salesOrderItem.SalesOrderItemInventoryAssignmentsWhereSalesOrderItem.FirstOrDefault();
        //                // if (inventoryAssignment != null)
        //                // {
        //                //    inventoryAssignment.Quantity += diff;
        //                // }
        //            }
        //        }
        //    }
        //}

        //public void AppsDepleteSalesOrders(IDerivation derivation)
        //{
        //    var salesOrderItems = this.Strategy.Session.Extent<SalesOrderItem>();
        //    salesOrderItems.Filter.AddEquals(M.SalesOrderItem.SalesOrderItemState, new SalesOrderItemStates(this.Strategy.Session).InProcess);
        //    salesOrderItems.Filter.AddExists(M.OrderItem.DeliveryDate);
        //    salesOrderItems.AddSort(M.OrderItem.DeliveryDate, SortDirection.Descending);

        //    salesOrderItems = this.Strategy.Session.Instantiate(salesOrderItems);

        //    var subtract = this.PreviousQuantityOnHand - this.QuantityOnHand;

        //    foreach (SalesOrderItem salesOrderItem in salesOrderItems)
        //    {
        //        if (subtract > 0 && salesOrderItem.QuantityRequestsShipping > 0)
        //        {
        //            decimal diff;
        //            if (subtract >= salesOrderItem.QuantityRequestsShipping)
        //            {
        //                diff = salesOrderItem.QuantityRequestsShipping;
        //            }
        //            else
        //            {
        //                diff = subtract;
        //            }

        //            subtract -= diff;

        //            var inventoryAssignment = salesOrderItem.SalesOrderItemInventoryAssignments.FirstOrDefault();
        //            if (inventoryAssignment != null)
        //            {
        //                inventoryAssignment.Quantity = diff;
        //            }
        //        }
        //    }
        //}

        //public void AppsOnDeriveUnitOfMeasure(IDerivation derivation)
        //{
        //    if (this.ExistPart)
        //    {
        //        this.UnitOfMeasure = this.Part.UnitOfMeasure;
        //    }
        //}

        public void AppsDelete(DeletableDelete method)
        {
            foreach (InventoryItemVersion version in this.AllVersions)
            {
                version.Delete();
            }
        }
    }
}