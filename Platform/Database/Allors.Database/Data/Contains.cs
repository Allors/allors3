// <copyright file="Contains.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Data
{
    using System.Collections.Generic;

    using Allors.Meta;
    using Allors.Protocol.Data;

    public class Contains : IPropertyPredicate
    {
        public Contains(IPropertyType propertyType = null) => this.PropertyType = propertyType;

        public IPropertyType PropertyType { get; set; }

        public IObject Object { get; set; }

        public string Argument { get; set; }

        public Predicate Save() =>
            new Predicate
            {
                Kind = PredicateKind.Contains,
                PropertyType = this.PropertyType?.Id,
                Object = this.Object?.Id.ToString(),
                Argument = this.Argument,
            };

        bool IPredicate.ShouldTreeShake(IDictionary<string, string> parameters) => ((IPredicate)this).HasMissingArguments(parameters);

        bool IPredicate.HasMissingArguments(IDictionary<string, string> parameters) => this.Argument != null && (parameters == null || !parameters.ContainsKey(this.Argument));

        void IPredicate.Build(ISession session, IDictionary<string, string> parameters, Allors.ICompositePredicate compositePredicate)
        {
            var containedObject = this.Argument != null ? session.GetObject(parameters[this.Argument]) : this.Object;

            if (this.PropertyType is IRoleType roleType)
            {
                compositePredicate.AddContains(roleType, containedObject);
            }
            else
            {
                var associationType = (IAssociationType)this.PropertyType;
                compositePredicate.AddContains(associationType, containedObject);
            }
        }
    }
}
