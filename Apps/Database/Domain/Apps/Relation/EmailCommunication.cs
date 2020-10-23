// <copyright file="EmailCommunication.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class EmailCommunication
    {
        // TODO: Cache
        public TransitionalConfiguration[] TransitionalConfigurations => new[] {
            new TransitionalConfiguration(this.M.EmailCommunication, this.M.EmailCommunication.CommunicationEventState),
        };

        public void AppsOnDerive(ObjectOnDerive method)
        {
            //if (!this.ExistSubject && this.ExistEmailTemplate && this.EmailTemplate.ExistSubjectTemplate)
            //{
            //    this.Subject = this.EmailTemplate.SubjectTemplate;
            //}

            //this.WorkItemDescription = $"Email to {this.ToEmail} about {this.Subject}";
        }
    }
}