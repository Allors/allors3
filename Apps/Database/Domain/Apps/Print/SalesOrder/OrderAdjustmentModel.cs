// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuoteItemModel.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Database.Domain.Print.SalesOrderModel
{
    using System.Globalization;

    public class OrderAdjustmentModel
    {
        public OrderAdjustmentModel(OrderAdjustment orderAdjustment)
        {
            var salesOrder = orderAdjustment.OrderWhereOrderAdjustment;

            this.AdjustmentTypeName = orderAdjustment.GetType().Name;
            this.Description = orderAdjustment.Description;

            if (orderAdjustment.GetType().Name.Equals(typeof(DiscountAdjustment).Name))
            {
                this.Amount = salesOrder.TotalDiscount.ToString("N2", new CultureInfo("nl-BE"));
            }

            if (orderAdjustment.GetType().Name.Equals(typeof(SurchargeAdjustment).Name))
            {
                this.Amount = salesOrder.TotalSurcharge.ToString("N2", new CultureInfo("nl-BE"));
            }

            if (orderAdjustment.GetType().Name.Equals(typeof(Fee).Name))
            {
                this.Amount = salesOrder.TotalFee.ToString("N2", new CultureInfo("nl-BE"));
            }

            if (orderAdjustment.GetType().Name.Equals(typeof(ShippingAndHandlingCharge).Name))
            {
                this.Amount = salesOrder.TotalShippingAndHandling.ToString("N2", new CultureInfo("nl-BE"));
            }

            if (orderAdjustment.GetType().Name.Equals(typeof(MiscellaneousCharge).Name))
            {
                var miscCharge = salesOrder.TotalExtraCharge - salesOrder.TotalFee - salesOrder.TotalShippingAndHandling;
                this.Amount = miscCharge.ToString("N2", new CultureInfo("nl-BE"));
            }
        }

        public string AdjustmentTypeName { get; set; }

        public string Amount { get; set; }

        public string Description { get; }
    }
}
