// <copyright file="RequestForInformation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class RequestForInformation
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.RequestForInformation, this.M.RequestForInformation.RequestState),
        };

        //public void AppsOnDerive(ObjectOnDerive method) => this.Sync(this.Strategy.Session);

        public void AppsOnPostDerive(ObjectOnPostDerive method)
        {
            //var deletePermission = new Permissions(this.Strategy.Session).Get(this.Meta.ObjectType, this.Meta.Delete);
            //if (this.IsDeletable())
            //{
            //    this.RemoveDeniedPermission(deletePermission);
            //}
            //else
            //{
            //    this.AddDeniedPermission(deletePermission);
            //}
        }

        private void Sync(ISession session)
        {
            // session.Prefetch(this.SyncPrefetch, this);
            //foreach (RequestItem requestItem in this.RequestItems)
            //{
            //    requestItem.Sync(this);
            //}
        }
    }
}