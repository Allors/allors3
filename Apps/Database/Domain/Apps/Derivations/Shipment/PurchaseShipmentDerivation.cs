// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Derivations;
    using Meta;
    using Database.Derivations;

    public class PurchaseShipmentDerivation : DomainDerivation
    {
        public PurchaseShipmentDerivation(M m) : base(m, new Guid("89A2FB27-6839-40D4-AFAB-79E25259B1C8")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(this.M.PurchaseShipment.ShipToParty),
                new ChangedPattern(this.M.PurchaseShipment.ShipFromParty),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            foreach (var @this in matches.Cast<PurchaseShipment>())
            {
                cycle.Validation.AssertExists(@this, @this.Meta.ShipFromParty);

                var shipToParty = @this.ShipToParty as InternalOrganisation;

                @this.ShipToAddress ??= @this.ShipToParty?.ShippingAddress ?? @this.ShipToParty?.GeneralCorrespondence as PostalAddress;

                if (!@this.ExistShipToFacility && shipToParty != null && shipToParty.StoresWhereInternalOrganisation.Count == 1)
                {
                    @this.ShipToFacility = shipToParty.StoresWhereInternalOrganisation.Single().DefaultFacility;
                }

                if (!@this.ExistShipmentNumber && shipToParty != null)
                {
                    var year = @this.Strategy.Session.Now().Year;
                    @this.ShipmentNumber = shipToParty.NextPurchaseShipmentNumber(year);

                    var fiscalYearInternalOrganisationSequenceNumbers = shipToParty.FiscalYearsInternalOrganisationSequenceNumbers.FirstOrDefault(v => v.FiscalYear == year);
                    var prefix = ((InternalOrganisation)@this.ShipToParty).CustomerShipmentSequence.IsEnforcedSequence ? ((InternalOrganisation)@this.ShipToParty).PurchaseShipmentNumberPrefix : fiscalYearInternalOrganisationSequenceNumbers.PurchaseShipmentNumberPrefix;
                    @this.SortableShipmentNumber = @this.Session().GetSingleton().SortableNumber(prefix, @this.ShipmentNumber, year.ToString());
                }

                if (!@this.ExistShipFromAddress && @this.ExistShipFromParty)
                {
                    @this.ShipFromAddress = @this.ShipFromParty.ShippingAddress;
                }

                if (@this.ShipmentItems.Any()
                    && @this.ShipmentItems.All(v => v.ExistShipmentReceiptWhereShipmentItem
                    && v.ShipmentReceiptWhereShipmentItem.QuantityAccepted.Equals(v.ShipmentReceiptWhereShipmentItem.OrderItem?.QuantityOrdered))
                    && @this.ShipmentItems.All(v => v.ShipmentItemState.Equals(new ShipmentItemStates(@this.Strategy.Session).Received)))
                {
                    @this.ShipmentState = new ShipmentStates(@this.Strategy.Session).Received;
                }

                // session.Prefetch(this.SyncPrefetch, this);
                foreach (ShipmentItem shipmentItem in @this.ShipmentItems)
                {
                    shipmentItem.Sync(@this);
                }
            }
        }
    }
}
