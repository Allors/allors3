// <copyright file="PullResponse.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Database.Pull
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class PullResponse : Response
    {
        [JsonPropertyName("accessControls")]
        public string[][] AccessControls { get; set; }

        [JsonPropertyName("namedCollections")]
        public Dictionary<string, string[]> NamedCollections { get; set; }

        [JsonPropertyName("namedObjects")]
        public Dictionary<string, string> NamedObjects { get; set; }

        [JsonPropertyName("namedValues")]
        public Dictionary<string, object> NamedValues { get; set; }

        [JsonPropertyName("objects")]
        public string[][] Objects { get; set; }
    }
}
