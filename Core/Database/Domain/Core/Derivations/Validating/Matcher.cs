// <copyright file="ChangedRoles.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IDomainDerivation type.</summary>

namespace Allors.Domain.Derivations.Validating
{
    using System;
    using System.Collections.Generic;
    using Meta;

    public class Matcher
    {
        private readonly IDictionary<IStrategy, ISet<IPropertyType>> propertyTypesByStrategies;

        public Matcher() => this.propertyTypesByStrategies = new Dictionary<IStrategy, ISet<IPropertyType>>();

        public void Add(IStrategy strategy, IPropertyType propertyType)
        {
            if (!this.propertyTypesByStrategies.TryGetValue(strategy, out var propertyTypes))
            {
                propertyTypes = new HashSet<IPropertyType>();
                this.propertyTypesByStrategies.Add(strategy, propertyTypes);
            }

            if (propertyType is RoleType roleType && roleType.ExistDefault)
            {
                propertyTypes.Add(roleType.Default);
            }
            else
            {
                propertyTypes.Add(propertyType);
            }
        }

        public void Match(IStrategy strategy, IPropertyType propertyType)
        {
            if (strategy.IsNewInSession)
            {
                return;
            }

            propertyType = propertyType is RoleType roleType && roleType.ExistDefault
                ? roleType.Default
                : propertyType;

            if (!this.propertyTypesByStrategies.TryGetValue(strategy, out var propertyTypes))
            {
                throw new Exception($"Could not match [{strategy}].{propertyType}");
            }

            if (!propertyTypes.Contains(propertyType))
            {
                throw new Exception($"Could not match [{strategy}].{propertyType}");
            }
        }
    }
}