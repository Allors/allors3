// <copyright file="PostalCodes.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class PostalCodes
    {
        private Cache<string, PostalCode> postalCodeByCode;

        public Cache<string, PostalCode> PostalCodeByCode => this.postalCodeByCode ??= new Cache<string, PostalCode>(this.Session, this.M.PostalCode.Code);
    }
}