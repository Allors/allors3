// <copyright file="RequestExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System.Linq;

    public static partial class RequestExtensions
    {
        public static bool IsDeletable(this Request @this) =>
            // EmailAddress is used whith anonymous request form website
            !@this.ExistEmailAddress
            && (@this.RequestState.Equals(new RequestStates(@this.Strategy.Session).Submitted)
                || @this.RequestState.Equals(new RequestStates(@this.Strategy.Session).Cancelled)
                || @this.RequestState.Equals(new RequestStates(@this.Strategy.Session).Rejected))
            && !@this.ExistQuoteWhereRequest
            && @this.RequestItems.All(v => v.IsDeletable);

        public static void AppsOnBuild(this Request @this, ObjectOnBuild method)
        {
            if (!@this.ExistRequestState && !@this.ExistOriginator)
            {
                @this.RequestState = new RequestStates(@this.Session()).Anonymous;
            }

            if (!@this.ExistRequestState && @this.ExistOriginator)
            {
                @this.RequestState = new RequestStates(@this.Session()).Submitted;
            }
        }

        public static void AppsOnPreDerive(this Request @this, ObjectOnPreDerive method)
        {
            //var (iteration, changeSet, derivedObjects) = method;

            //if (iteration.IsMarked(@this) || changeSet.IsCreated(@this) || changeSet.HasChangedRoles(@this))
            //{
            //    foreach (RequestItem requestItem in @this.RequestItems)
            //    {
            //        iteration.AddDependency(requestItem, @this);
            //        iteration.Mark(requestItem);
            //    }
            //}
        }

        public static void AppsOnDerive(this Request @this, ObjectOnDerive method)
        {
            //var session = @this.Strategy.Session;
            //if (!@this.ExistRecipient)
            //{
            //    var internalOrganisations = new Organisations(session).InternalOrganisations();

            //    if (internalOrganisations.Count() == 1)
            //    {
            //        @this.Recipient = internalOrganisations.Single();
            //    }
            //}

            //if (@this.ExistRecipient && !@this.ExistRequestNumber)
            //{
            //    @this.RequestNumber = @this.Recipient.NextRequestNumber(session.Now().Year);
            //    (@this).SortableRequestNumber = @this.Session().GetSingleton().SortableNumber(@this.Recipient.RequestNumberPrefix, @this.RequestNumber, @this.RequestDate.Year.ToString());
            //}

            //@this.DeriveInitialObjectState();


            //@this.AddSecurityToken(new SecurityTokens(session).DefaultSecurityToken);
        }

        public static void AppsDelete(this Request @this, DeletableDelete method)
        {
            if (@this.IsDeletable())
            {
                foreach (RequestItem item in @this.RequestItems)
                {
                    item.Delete();
                }
            }
        }

        public static void AppsCancel(this Request @this, RequestCancel method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Cancelled;

        public static void AppsReject(this Request @this, RequestReject method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Rejected;

        public static void AppsSubmit(this Request @this, RequestSubmit method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Submitted;

        public static void AppsHold(this Request @this, RequestHold method) => @this.RequestState = new RequestStates(@this.Strategy.Session).PendingCustomer;
    }
}