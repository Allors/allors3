// <copyright file="Domain.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Database.Derivations;
    using Meta;

    public class RequestDerivation : DomainDerivation
    {
        public RequestDerivation(M m) : base(m, new Guid("AF5D09BF-9ACF-4C29-9445-6D24BE2F04E6")) =>
            this.Patterns = new Pattern[]
            {
                new ChangedPattern(this.M.Request.Recipient),
                new ChangedPattern(this.M.Request.AssignedCurrency),
            };

        public override void Derive(IDomainDerivationCycle cycle, IEnumerable<IObject> matches)
        {
            var session = cycle.Session;

            foreach (var @this in matches.Cast<Request>())
            {
                if (@this.ExistRecipient && !@this.ExistRequestNumber)
                {
                    var year = @this.RequestDate.Year;
                    @this.RequestNumber = @this.Recipient.NextRequestNumber(year);

                    var fiscalYearInternalOrganisationSequenceNumbers = @this.Recipient.FiscalYearsInternalOrganisationSequenceNumbers.FirstOrDefault(v => v.FiscalYear == year);
                    var prefix = @this.Recipient.RequestSequence.IsEnforcedSequence ? @this.Recipient.RequestNumberPrefix : fiscalYearInternalOrganisationSequenceNumbers.RequestNumberPrefix;
                    @this.SortableRequestNumber = @this.Session().GetSingleton().SortableNumber(prefix, @this.RequestNumber, year.ToString());
                }

                @this.DerivedCurrency = @this.AssignedCurrency ?? @this.Originator?.PreferredCurrency ?? @this.Recipient?.PreferredCurrency;

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

                @this.AddSecurityToken(new SecurityTokens(session).DefaultSecurityToken);
            }
        }
    }
}
