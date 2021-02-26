// <copyright file="WorkEffortInventoryAssignment.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System.Linq;
    using Resources;

    public partial class WorkEffortInventoryAssignment
    {
        public void AppsDelete(DeletableDelete method)
        {
            var transaction = this.strategy.Transaction;
            var derivation = new Derivations.Default.DefaultDerivation(transaction);
            this.SyncInventoryTransactions(derivation, this.InventoryItem, this.Quantity, new InventoryTransactionReasons(transaction).Consumption, true);
        }

        public void AppsDelegateAccess(DelegatedAccessControlledObjectDelegateAccess method)
        {
            if (method.SecurityTokens == null)
            {
                method.SecurityTokens = this.Assignment?.SecurityTokens.ToArray();
            }

            if (method.DeniedPermissions == null)
            {
                method.DeniedPermissions = this.Assignment?.DeniedPermissions.ToArray();
            }
        }

        public void SyncInventoryTransactions(IDerivation derivation, InventoryItem inventoryItem, decimal initialQuantity, InventoryTransactionReason reason, bool isCancellation)
        {
            // TODO: Move sync to new derivations  

            var adjustmentQuantity = 0M;
            var existingQuantity = 0M;
            var matchingTransactions = this.InventoryItemTransactions
                .Where(t => t.Reason.Equals(reason) && t.Part.Equals(inventoryItem.Part)).ToArray();

            if (matchingTransactions.Length > 0)
            {
                existingQuantity = matchingTransactions.Sum(t => t.Quantity);
            }

            if (isCancellation)
            {
                adjustmentQuantity = 0 - existingQuantity;
            }
            else
            {
                adjustmentQuantity = initialQuantity - existingQuantity;

                if (inventoryItem is NonSerialisedInventoryItem nonserialisedInventoryItem && nonserialisedInventoryItem.QuantityOnHand < adjustmentQuantity)
                {
                    derivation.Validation.AddError(this, this.M.NonSerialisedInventoryItem.QuantityOnHand, ErrorMessages.InsufficientStock);
                }
            }

            if (adjustmentQuantity != 0)
            {
                this.AddInventoryItemTransaction(new InventoryItemTransactionBuilder(this.Transaction())
                    .WithPart(inventoryItem.Part)
                    .WithFacility(inventoryItem.Facility)
                    .WithQuantity(adjustmentQuantity)
                    .WithCost(inventoryItem.Part.PartWeightedAverage.AverageCost)
                    .WithReason(reason)
                    .Build());
            }
        }
    }
}
