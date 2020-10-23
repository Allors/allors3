// <copyright file="Settingses.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;

    public partial class Settingses
    {
        protected override void AppsPrepare(Setup setup)
        {
            setup.AddDependency(this.ObjectType, this.M.Singleton);
            setup.AddDependency(this.ObjectType, this.M.InventoryStrategy);
            setup.AddDependency(this.ObjectType, this.M.Currency);
        }

        protected override void AppsSetup(Setup setup)
        {
            var singleton = this.Session.GetSingleton();
            singleton.Settings ??= new SettingsBuilder(this.Session)
                .WithUseProductNumberCounter(true)
                .WithUsePartNumberCounter(true)
                .Build();

            var settings = singleton.Settings;

            var inventoryStrategy = new InventoryStrategies(this.Session).Standard;
            var preferredCurrency = new Currencies(this.Session).FindBy(this.M.Currency.IsoCode, "EUR");

            settings.InventoryStrategy ??= inventoryStrategy;
            settings.SkuPrefix ??= "Sku-";
            settings.SerialisedItemPrefix ??= "S-";
            settings.ProductNumberPrefix ??= "Art-";
            settings.PartNumberPrefix ??= "Part-";
            settings.PreferredCurrency ??= preferredCurrency;

            settings.SkuCounter ??= new CounterBuilder(this.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            settings.SerialisedItemCounter ??= new CounterBuilder(this.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            settings.ProductNumberCounter ??= new CounterBuilder(this.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
            settings.PartNumberCounter ??= new CounterBuilder(this.Session).WithUniqueId(Guid.NewGuid()).WithValue(0).Build();
        }
    }
}