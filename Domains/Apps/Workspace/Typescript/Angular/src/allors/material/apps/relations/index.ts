import { LayoutComponent } from './layout.component';
import { CommunicationEventsTableComponent } from './communicationEvents/communicationEvents.table.component';

// Routing
import { EmailAddressAddComponent } from './contactMechanisms/contactMechanism/emailAddressAdd.component';
import { EmailAddressEditComponent } from './contactMechanisms/contactMechanism/emailAddressEdit.component';
import { OrganisationAddContactComponent } from './organisations/organisation/organisationAddContact.component';
import { OrganisationEditContactComponent } from './organisations/organisation/organisationEditContact.component';
import { OrganisationFormComponent } from './organisations/organisation/organisation.component';
import { OrganisationOverviewComponent } from './organisations/organisation/organisation-overview.component';
import { OrganisationsComponent } from './organisations/organisations.component';
import { PeopleComponent } from './people/people.component';
import { PersonFormComponent } from './people/person/person.component';
import { PersonOverviewComponent } from './people/person/person-overview.component';
import { PostalAddressAddComponent } from './contactMechanisms/contactMechanism/postalAddressAdd.component';
import { PostalAddressEditComponent } from './contactMechanisms/contactMechanism/postalAddressEdit.component';
import { RelationDashboardComponent } from './dashboard/relation-dashboard.component';
import { TelecommunicationsNumberAddComponent } from './contactMechanisms/contactMechanism/telecommunicationsNumberAdd.component';
import { TelecommunicationsNumberEditComponent } from './contactMechanisms/contactMechanism/telecommunicationsNumberEdit.component';
import { WebAddressAddComponent } from './contactMechanisms/contactMechanism/webAddressAdd.component';
import { WebAddressEditComponent } from './contactMechanisms/contactMechanism/webAddressEdit.component';

export const RELATIONS: any[] = [
  LayoutComponent,
  CommunicationEventsTableComponent,
];

export const RELATIONS_ROUTING: any[] = [
  EmailAddressAddComponent, EmailAddressEditComponent,
  OrganisationAddContactComponent, OrganisationEditContactComponent, OrganisationFormComponent, OrganisationOverviewComponent, OrganisationsComponent,
  PeopleComponent, PersonFormComponent, PersonOverviewComponent,
  PostalAddressAddComponent, PostalAddressEditComponent,
  RelationDashboardComponent,
  TelecommunicationsNumberAddComponent, TelecommunicationsNumberEditComponent,
  WebAddressAddComponent, WebAddressEditComponent,
];

export {
  CommunicationEventsTableComponent,
  // Routing
  EmailAddressAddComponent,
  EmailAddressEditComponent,
  OrganisationAddContactComponent,
  OrganisationEditContactComponent,
  OrganisationFormComponent,
  OrganisationOverviewComponent,
  OrganisationsComponent,
  PeopleComponent,
  PersonFormComponent,
  PersonOverviewComponent,
  PostalAddressAddComponent,
  PostalAddressEditComponent,
  RelationDashboardComponent,
  TelecommunicationsNumberAddComponent,
  TelecommunicationsNumberEditComponent,
  WebAddressAddComponent,
  WebAddressEditComponent,
};
