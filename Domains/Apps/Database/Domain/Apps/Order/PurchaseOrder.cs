// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PurchaseOrder.cs" company="Allors bvba">
//   Copyright 2002-2012 Allors bvba.
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using Meta;
    using Resources;

    public partial class PurchaseOrder
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.PurchaseOrder.PurchaseOrderState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        public bool IsProvisional => this.PurchaseOrderState.Equals(new PurchaseOrderStates(this.Strategy.Session).Provisional);

        public OrderItem[] OrderItems => this.PurchaseOrderItems;

        public void AppsOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistPurchaseOrderState)
            {
                this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).Provisional;
            }

            if (this.ExistTakenViaSupplier)
            {
                this.PreviousTakenViaSupplier = this.TakenViaSupplier;
            }

            if (!this.ExistOrderNumber)
            {
                this.OrderNumber = Singleton.Instance(this).InternalOrganisation.DeriveNextPurchaseOrderNumber();
            }

            if (!this.ExistOrderDate)
            {
                this.OrderDate = DateTime.UtcNow;
            }

            if (!this.ExistEntryDate)
            {
                this.EntryDate = DateTime.UtcNow;
            }

            if (!this.ExistCustomerCurrency)
            {
                this.CustomerCurrency = Singleton.Instance(this).PreferredCurrency;
            }

            if (!this.ExistFacility)
            {
                this.Facility = Singleton.Instance(this).InternalOrganisation.DefaultFacility;
            }
        }

        public void AppsOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            if (derivation.HasChangedRoles(this))
            {
                derivation.AddDependency(this, Singleton.Instance(this));
                derivation.AddDependency(this, Singleton.Instance(this));
                derivation.AddDependency(this, this.TakenViaSupplier);
            }

            foreach (PurchaseOrderItem orderItem in this.OrderItems)
            {
                derivation.AddDependency(this, orderItem);
            }
        }

        public void AppsOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;


            Organisation supplier = this.TakenViaSupplier as Organisation;
            if (supplier != null)
            {
                if (!Singleton.Instance(this.strategy.Session).InternalOrganisation.ActiveSuppliers.Contains(supplier))
                {
                    derivation.Validation.AddError(this, this.Meta.TakenViaSupplier, ErrorMessages.PartyIsNotASupplier);
                }
            }

            if (!this.ExistShipToAddress)
            {
                this.ShipToAddress = Singleton.Instance(this.strategy.Session).InternalOrganisation.ShippingAddress;
            }

            if (!this.ExistBillToContactMechanism)
            {
                this.BillToContactMechanism = Singleton.Instance(this.strategy.Session).InternalOrganisation.BillingAddress;
            }

            if (!this.ExistTakenViaContactMechanism && this.ExistTakenViaSupplier)
            {
                this.TakenViaContactMechanism = this.TakenViaSupplier.OrderAddress;
            }

            this.AppsOnDeriveOrderItems(derivation);
            this.AppsOnDeriveCurrentOrderStatus(derivation);
            this.AppsOnDeriveLocale(derivation);
            this.AppsOnDeriveOrderTotals(derivation);

            this.PreviousTakenViaSupplier = this.TakenViaSupplier;
        }

        public void AppsCancel(OrderCancel method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).Cancelled;
        }

        public void AppsConfirm(OrderConfirm method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).InProcess;
        }

        public void AppsReject(OrderReject method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).Rejected;
        }

        public void AppsHold(OrderHold method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).OnHold;
        }

        public void AppsApprove(OrderApprove method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).RequestsApproval;
        }

        public void AppsContinue(OrderContinue method)
        {
            this.PurchaseOrderState = new PurchaseOrderStates(this.Strategy.Session).InProcess;
        }

        public void AppsOnDeriveCurrentOrderStatus(IDerivation derivation)
        {
            // TODO: State Transitions
            //if (this.ExistCurrentShipmentStateVersion && this.CurrentShipmentStateVersion.CurrentObjectState.Equals(new PurchaseOrderStates(this.Strategy.Session).Received))
            //{
            //    this.Complete();
            //}

            //if (this.ExistCurrentPaymentStateVersion && this.CurrentPaymentStateVersion.CurrentObjectState.Equals(new PurchaseOrderStates(this.Strategy.Session).Paid))
            //{
            //    this.Finish();
            //}
        }

        public void AppsOnDeriveLocale(IDerivation derivation)
        {
            // TODO: ???
            //this.Locale = this.ExistShipToBuyer && this.ShipToBuyer.ExistLocale
            //                  ? this.ShipToBuyer.Locale
            //                  : Singleton.Instance(this.Strategy.Session).DefaultLocale;
        }

        public void AppsOnDeriveOrderTotals(IDerivation derivation)
        {
            if (this.ExistValidOrderItems)
            {
                this.TotalBasePrice = 0;
                this.TotalDiscount = 0;
                this.TotalSurcharge = 0;
                this.TotalVat = 0;
                this.TotalExVat = 0;
                this.TotalIncVat = 0;

                foreach (PurchaseOrderItem orderItem in this.ValidOrderItems)
                {
                    this.TotalBasePrice += orderItem.TotalBasePrice;
                    this.TotalDiscount += orderItem.TotalDiscount;
                    this.TotalSurcharge += orderItem.TotalSurcharge;
                    this.TotalVat += orderItem.TotalVat;
                    this.TotalExVat += orderItem.TotalExVat;
                    this.TotalIncVat += orderItem.TotalIncVat;
                }
            }
        }

        public void AppsOnDeriveOrderItems(IDerivation derivation)
        {
            var quantityOrderedByProduct = new Dictionary<Product, decimal>();
            var totalBasePriceByProduct = new Dictionary<Product, decimal>();
            var quantityOrderedByPart = new Dictionary<Part, decimal>();
            var totalBasePriceByPart = new Dictionary<Part, decimal>();

            foreach (PurchaseOrderItem purchaseOrderItem in this.ValidOrderItems)
            {
                purchaseOrderItem.OnDerive(x => x.WithDerivation(derivation));
                purchaseOrderItem.AppsOnDeriveDeliveryDate(derivation);
                purchaseOrderItem.AppsOnDeriveCurrentShipmentStatus(derivation);
                purchaseOrderItem.AppsOnDerivePrices();
                purchaseOrderItem.AppsDeriveVatRegime(derivation);


                if (purchaseOrderItem.ExistProduct)
                {
                    if (!quantityOrderedByProduct.ContainsKey(purchaseOrderItem.Product))
                    {
                        quantityOrderedByProduct.Add(purchaseOrderItem.Product, purchaseOrderItem.QuantityOrdered);
                        totalBasePriceByProduct.Add(purchaseOrderItem.Product, purchaseOrderItem.TotalBasePrice);
                    }
                    else
                    {
                        quantityOrderedByProduct[purchaseOrderItem.Product] += purchaseOrderItem.QuantityOrdered;
                        totalBasePriceByProduct[purchaseOrderItem.Product] += purchaseOrderItem.TotalBasePrice;
                    }
                }

                if (purchaseOrderItem.ExistPart)
                {
                    if (!quantityOrderedByPart.ContainsKey(purchaseOrderItem.Part))
                    {
                        quantityOrderedByPart.Add(purchaseOrderItem.Part, purchaseOrderItem.QuantityOrdered);
                        totalBasePriceByPart.Add(purchaseOrderItem.Part, purchaseOrderItem.TotalBasePrice);
                    }
                    else
                    {
                        quantityOrderedByPart[purchaseOrderItem.Part] += purchaseOrderItem.QuantityOrdered;
                        totalBasePriceByPart[purchaseOrderItem.Part] += purchaseOrderItem.TotalBasePrice;
                    }
                }
            }
        }
    }
}