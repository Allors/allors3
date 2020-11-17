// <copyright file="Step.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Json
{
    using System;
    using System.Text.Json.Serialization;

    public class Step : IVisitable
    {
        [JsonPropertyName("associationType")]
        public Guid? AssociationType { get; set; }

        [JsonPropertyName("roleType")]
        public Guid? RoleType { get; set; }

        [JsonPropertyName("next")]
        public Step Next { get; set; }

        [JsonPropertyName("include")]
        public Node[] Include { get; set; }

        public void Accept(IVisitor visitor) => visitor.VisitStep(this);
    }
}