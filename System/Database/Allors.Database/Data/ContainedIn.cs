// <copyright file="ContainedIn.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Data
{
    using System.Collections.Generic;
    using Allors.Database.Meta;
    
    public class ContainedIn : IPropertyPredicate
    {
        public string[] Dependencies { get; set; }

        public ContainedIn(IPropertyType propertyType = null) => this.PropertyType = propertyType;

        public IPropertyType PropertyType { get; set; }

        public IExtent Extent { get; set; }

        public IEnumerable<IObject> Objects { get; set; }

        public string Parameter { get; set; }

        bool IPredicate.ShouldTreeShake(IDictionary<string, string> parameters) => this.HasMissingDependencies(parameters) || this.HasMissingArguments(parameters);

        bool IPredicate.HasMissingArguments(IDictionary<string, string> parameters) => this.HasMissingArguments(parameters);

        void IPredicate.Build(ISession session, IDictionary<string, string> parameters, Database.ICompositePredicate compositePredicate)
        {
            if (!this.HasMissingArguments(parameters))
            {
                var objects = this.Parameter != null ? session.GetObjects(parameters[this.Parameter]) : this.Objects;

                if (this.PropertyType is IRoleType roleType)
                {
                    if (objects != null)
                    {
                        compositePredicate.AddContainedIn(roleType, objects);
                    }
                    else
                    {
                        compositePredicate.AddContainedIn(roleType, this.Extent.Build(session, parameters));
                    }
                }
                else
                {
                    var associationType = (IAssociationType)this.PropertyType;
                    if (objects != null)
                    {
                        compositePredicate.AddContainedIn(associationType, objects);
                    }
                    else
                    {
                        compositePredicate.AddContainedIn(associationType, this.Extent.Build(session, parameters));
                    }
                }
            }
        }

        private bool HasMissingArguments(IDictionary<string, string> parameters) => (this.Parameter != null && (parameters?.ContainsKey(this.Parameter) == false)) || this.Extent?.HasMissingArguments(parameters) == true;

        public void Accept(IVisitor visitor) => visitor.VisitContainedIn(this);
    }
}
