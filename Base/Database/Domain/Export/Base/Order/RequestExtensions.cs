// <copyright file="RequestExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using System.Runtime.CompilerServices;

namespace Allors.Domain
{
    using System;

    using Allors.Meta;

    public static partial class RequestExtensions
    {
        public static void BaseOnBuild(this Request @this, ObjectOnBuild method) => @this.AddSecurityToken(@this.Strategy.Session.GetSingleton().InitialSecurityToken);

        public static void BaseOnDerive(this Request @this, ObjectOnDerive method)
        {
            if (!@this.ExistRecipient)
            {
                var internalOrganisations = new Organisations(@this.Strategy.Session).InternalOrganisations();

                if (internalOrganisations.Count() == 1)
                {
                    @this.Recipient = internalOrganisations.Single();
                }
            }

            if (@this.ExistRecipient && !@this.ExistRequestNumber)
            {
                @this.RequestNumber = @this.Recipient.NextRequestNumber(@this.Strategy.Session.Now().Year);
            }

            @this.DeriveInitialObjectState();

            var singleton = @this.Strategy.Session.GetSingleton();

            @this.SecurityTokens = new[]
            {
                singleton.DefaultSecurityToken,
            };

            if (@this.ExistRecipient)
            {
                @this.AddSecurityToken(@this.Recipient.LocalAdministratorSecurityToken);
            }
        }

        public static void DeriveInitialObjectState(this Request @this)
        {
            var session = @this.Strategy.Session;
            if (!@this.ExistRequestState && !@this.ExistOriginator)
            {
                @this.RequestState = new RequestStates(session).Anonymous;
            }

            if (!@this.ExistRequestState && @this.ExistOriginator)
            {
                @this.RequestState = new RequestStates(session).Submitted;
            }

            if (@this.ExistRequestState && @this.RequestState.Equals(new RequestStates(session).Anonymous) && @this.ExistOriginator)
            {
                @this.RequestState = new RequestStates(session).Submitted;

                if (@this.ExistEmailAddress
                    && @this.Originator.PartyContactMechanisms.Where(v => v.ContactMechanism.GetType().Name == typeof(EmailAddress).Name).FirstOrDefault(v => ((EmailAddress)v.ContactMechanism).ElectronicAddressString.Equals(@this.EmailAddress)) == null)
                {
                    @this.Originator.AddPartyContactMechanism(
                        new PartyContactMechanismBuilder(session)
                        .WithContactMechanism(new EmailAddressBuilder(session).WithElectronicAddressString(@this.EmailAddress).Build())
                        .WithContactPurpose(new ContactMechanismPurposes(session).GeneralEmail)
                        .Build());
                }

                if (@this.ExistTelephoneNumber
                    && @this.Originator.PartyContactMechanisms.Where(v => v.ContactMechanism.GetType().Name == typeof(TelecommunicationsNumber).Name).FirstOrDefault(v => ((TelecommunicationsNumber)v.ContactMechanism).ContactNumber.Equals(@this.TelephoneNumber)) == null)
                {
                    @this.Originator.AddPartyContactMechanism(
                        new PartyContactMechanismBuilder(session)
                            .WithContactMechanism(new TelecommunicationsNumberBuilder(session).WithContactNumber(@this.TelephoneNumber).WithCountryCode(@this.TelephoneCountryCode).Build())
                            .WithContactPurpose(new ContactMechanismPurposes(session).GeneralPhoneNumber)
                            .Build());
                }
            }
        }

        public static void BaseCancel(this Request @this, RequestCancel method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Cancelled;

        public static void BaseReject(this Request @this, RequestReject method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Rejected;

        public static void BaseSubmit(this Request @this, RequestSubmit method) => @this.RequestState = new RequestStates(@this.Strategy.Session).Submitted;

        public static void BaseHold(this Request @this, RequestHold method) => @this.RequestState = new RequestStates(@this.Strategy.Session).PendingCustomer;
    }
}
