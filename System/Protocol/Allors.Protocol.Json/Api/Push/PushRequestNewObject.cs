// <copyright file="PushRequestNewObject.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Json.Api.Push
{
    using System.Text.Json.Serialization;

    /// <summary>
    ///  New objects require NI and T.
    ///  Existing objects require I and V.
    /// </summary>
    public class PushRequestNewObject
    {
        [JsonPropertyName("ni")]
        public string NewWorkspaceId { get; set; }

        [JsonPropertyName("t")]
        public int ObjectType { get; set; }

        [JsonPropertyName("roles")]
        public PushRequestRole[] Roles { get; set; }
    }
}
