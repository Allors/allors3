// <copyright file="CommunicationEventExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Linq;
    using Allors.Meta;
    using Resources;

    public static partial class CommunicationEventExtensions
    {
        public static DateTime? BaseGetStart(this CommunicationEvent communicationEvent)
        {
            if (communicationEvent.ExistActualStart)
            {
                return communicationEvent.ActualStart;
            }

            if (communicationEvent.ExistScheduledStart)
            {
                return communicationEvent.ScheduledStart;
            }

            return null;
        }

        public static void BaseOnDerive(this CommunicationEvent @this, ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            if (!@this.ExistOwner && @this.Strategy.Session.GetUser() is Person owner)
            {
                @this.Owner = owner;
            }

            if (@this.ExistScheduledStart && @this.ExistScheduledEnd && @this.ScheduledEnd < @this.ScheduledStart)
            {
                derivation.Validation.AddError(@this, M.CommunicationEvent.ScheduledEnd, ErrorMessages.EndDateBeforeStartDate);
            }

            if (@this.ExistActualStart && @this.ExistActualEnd && @this.ActualEnd < @this.ActualStart)
            {
                derivation.Validation.AddError(@this, M.CommunicationEvent.ActualEnd, ErrorMessages.EndDateBeforeStartDate);
            }

            if (!@this.ExistCommunicationEventState)
            {
                if (!@this.ExistActualStart || (@this.ExistActualStart && @this.ActualStart > @this.Strategy.Session.Now()))
                {
                    @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).Scheduled;
                }

                if (@this.ExistActualStart && @this.ActualStart <= @this.Strategy.Session.Now() &&
                    ((@this.ExistActualEnd && @this.ActualEnd > @this.Strategy.Session.Now()) || !@this.ExistActualEnd))
                {
                    @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).InProgress;
                }

                if (@this.ExistActualEnd && @this.ActualEnd <= @this.Strategy.Session.Now())
                {
                    @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).Completed;
                }
            }

            if (!@this.ExistInitialScheduledStart && @this.ExistScheduledStart)
            {
                @this.InitialScheduledStart = @this.ScheduledStart;
            }

            if (!@this.ExistInitialScheduledEnd && @this.ExistScheduledEnd)
            {
                @this.InitialScheduledEnd = @this.ScheduledEnd;
            }

            @this.DeriveInvolvedParties();
        }

        public static void BaseOnPostDerive(this CommunicationEvent @this, ObjectOnPostDerive method)
        {
            var session = @this.Strategy.Session;
            @this.AddSecurityToken(new SecurityTokens(session).DefaultSecurityToken);
            @this.AddSecurityToken(@this.Owner?.OwnerSecurityToken);
        }

        public static void BaseDelete(this CommunicationEvent @this, DeletableDelete method) => @this.RemoveWorkEfforts();

        public static void BaseClose(this CommunicationEvent @this, CommunicationEventClose method) => @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).Completed;

        public static void BaseReopen(this CommunicationEvent @this, CommunicationEventReopen method) => @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).Scheduled;

        public static void BaseCancel(this CommunicationEvent @this, CommunicationEventCancel method) => @this.CommunicationEventState = new CommunicationEventStates(@this.Strategy.Session).Cancelled;

        private static void DeriveInvolvedParties(this CommunicationEvent @this)
        {
            var derivedRoles = (CommunicationEventDerivedRoles)@this;

            var partiesToRemove = @this.PartiesWhereCommunicationEvent.ToList();

            derivedRoles.RemoveInvolvedParties();

            if (@this.ExistFromParty)
            {
                derivedRoles.AddInvolvedParty(@this.FromParty);

                // HACK: DerivedRoles
                var fromPartyDerivedRoles = (PartyDerivedRoles)@this.FromParty;

                fromPartyDerivedRoles.AddCommunicationEvent(@this);
                if (partiesToRemove.Contains(@this.FromParty))
                {
                    partiesToRemove.Remove(@this.FromParty);
                }
            }

            if (@this.ExistToParty)
            {
                derivedRoles.AddInvolvedParty(@this.ToParty);

                // HACK: DerivedRoles
                var toPartyDerivedRoles = (PartyDerivedRoles)@this.ToParty;

                toPartyDerivedRoles.AddCommunicationEvent(@this);
                if (partiesToRemove.Contains(@this.ToParty))
                {
                    partiesToRemove.Remove(@this.ToParty);
                }
            }

            if (@this.ExistOwner)
            {
                derivedRoles.AddInvolvedParty(@this.Owner);

                // HACK: DerivedRoles
                var ownerDerivedRoles = (PartyDerivedRoles)@this.Owner;

                ownerDerivedRoles.AddCommunicationEvent(@this);
                if (partiesToRemove.Contains(@this.Owner))
                {
                    partiesToRemove.Remove(@this.Owner);
                }
            }

            foreach (Party party in @this.InvolvedParties)
            {
                if (party is Person person)
                {
                    foreach (OrganisationContactRelationship organisationContactRelationship in person.OrganisationContactRelationshipsWhereContact)
                    {
                        if (organisationContactRelationship.FromDate <= @this.Strategy.Session.Now() &&
                            (!organisationContactRelationship.ExistThroughDate || organisationContactRelationship.ThroughDate >= @this.Strategy.Session.Now()))
                        {
                            var organisation = organisationContactRelationship.Organisation;

                            derivedRoles.AddInvolvedParty(organisation);

                            // HACK: DerivedRoles
                            var organisationDerivedRoles = (PartyDerivedRoles)organisation;

                            organisationDerivedRoles.AddCommunicationEvent(@this);
                            if (partiesToRemove.Contains(organisation))
                            {
                                partiesToRemove.Remove(organisation);
                            }
                        }
                    }
                }
            }

            foreach (var party in partiesToRemove)
            {
                // HACK: DerivedRoles
                var partyDerivedRoles = (PartyDerivedRoles)party;

                partyDerivedRoles.RemoveCommunicationEvent(@this);
            }
        }
    }
}
