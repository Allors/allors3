// <copyright file="SyncResponse.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Json.Api.Security
{
    using System.Text.Json.Serialization;
    using Push;

    public class SecurityResponse
    {
        [JsonPropertyName("accessControls")]
        public SecurityResponseAccessControl[] AccessControls { get; set; }

        [JsonPropertyName("permissions")]
        public object[][] Permissions { get; set; }
    }
}
