// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { DatabaseObject
, Method } from '@allors/workspace/core';

import { User } from './User.g';
import { Deletable } from './Deletable.g';
import { UniquelyIdentifiable } from './UniquelyIdentifiable.g';
import { Organisation } from './Organisation.g';
import { Permission } from './Permission.g';
import { Data } from './Data.g';
import { SessionOrganisation } from './SessionOrganisation.g';
import { WorkspaceOrganisation } from './WorkspaceOrganisation.g';

export interface Person extends DatabaseObject
, User {

    CanReadFirstName: boolean;
    CanWriteFirstName: boolean;
    FirstName: string | null;

    CanReadMiddleName: boolean;
    CanWriteMiddleName: boolean;
    MiddleName: string | null;

    CanReadLastName: boolean;
    CanWriteLastName: boolean;
    LastName: string | null;

    CanReadBirthDate: boolean;
    CanWriteBirthDate: boolean;
    BirthDate: string | null;

    CanReadFullName: boolean;
    FullName: string | null;

    CanReadDomainFullName: boolean;
    CanWriteDomainFullName: boolean;
    DomainFullName: string | null;

    CanReadDomainGreeting: boolean;
    CanWriteDomainGreeting: boolean;
    DomainGreeting: string | null;

    CanReadIsStudent: boolean;
    CanWriteIsStudent: boolean;
    IsStudent: boolean | null;

    CanReadWeight: boolean;
    CanWriteWeight: boolean;
    Weight: string | null;

    CanReadCycleOne: boolean;
    CanWriteCycleOne: boolean;
    CycleOne: Organisation | null;

    CanReadCycleMany: boolean;
    CanWriteCycleMany: boolean;
    CycleMany: Organisation[];
    AddCycleMany(value: Organisation) : void;
    RemoveCycleMany(value: Organisation) : void;


    WorkspaceFullName: string | null;


    SessionFullName: string | null;


    DatasWhereAutocompleteFilter : Data[];


    DatasWhereAutocompleteOptions : Data[];


    DataWhereChip : Data | null;


    OrganisationWhereEmployee : Organisation | null;


    OrganisationWhereManager : Organisation | null;


    OrganisationsWhereOwner : Organisation[];


    OrganisationsWhereShareholder : Organisation[];


    OrganisationsWhereCycleOne : Organisation[];


    OrganisationsWhereCycleMany : Organisation[];


    SessionOrganisationWhereSessionDatabaseEmployee : SessionOrganisation | null;


    SessionOrganisationWhereSessionDatabaseManager : SessionOrganisation | null;


    SessionOrganisationsWhereSessionDatabaseOwner : SessionOrganisation[];


    SessionOrganisationsWhereSessionDatabaseShareholder : SessionOrganisation[];


    WorkspaceOrganisationWhereWorkspaceDatabaseEmployee : WorkspaceOrganisation | null;


    WorkspaceOrganisationWhereWorkspaceDatabaseManager : WorkspaceOrganisation | null;


    WorkspaceOrganisationsWhereWorkspaceDatabaseOwner : WorkspaceOrganisation[];


    WorkspaceOrganisationsWhereWorkspaceDatabaseShareholder : WorkspaceOrganisation[];


    CanExecuteDelete: boolean;
    Delete: Method;

}