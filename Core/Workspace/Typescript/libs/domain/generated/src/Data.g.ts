// Allors generated file.
// Do not edit this file, changes will be overwritten.
/* tslint:disable */
import { DatabaseObject
, Method } from '@allors/workspace/core';

import { Person } from './Person.g';
import { Permission } from './Permission.g';
import { Organisation } from './Organisation.g';

export interface Data extends DatabaseObject
 {

    CanReadAutocompleteFilter: boolean;
    CanWriteAutocompleteFilter: boolean;
    AutocompleteFilter: Person | null;

    CanReadAutocompleteOptions: boolean;
    CanWriteAutocompleteOptions: boolean;
    AutocompleteOptions: Person | null;

    CanReadCheckbox: boolean;
    CanWriteCheckbox: boolean;
    Checkbox: boolean | null;

    CanReadChips: boolean;
    CanWriteChips: boolean;
    Chips: Person[];
    AddChip(value: Person) : void;
    RemoveChip(value: Person) : void;

    CanReadString: boolean;
    CanWriteString: boolean;
    String: string | null;

    CanReadDecimal: boolean;
    CanWriteDecimal: boolean;
    Decimal: string | null;

    CanReadDate: boolean;
    CanWriteDate: boolean;
    Date: string | null;

    CanReadDateTime: boolean;
    CanWriteDateTime: boolean;
    DateTime: string | null;

    CanReadDateTime2: boolean;
    CanWriteDateTime2: boolean;
    DateTime2: string | null;

    CanReadRadioGroup: boolean;
    CanWriteRadioGroup: boolean;
    RadioGroup: string | null;

    CanReadSlider: boolean;
    CanWriteSlider: boolean;
    Slider: number | null;

    CanReadSlideToggle: boolean;
    CanWriteSlideToggle: boolean;
    SlideToggle: boolean | null;

    CanReadPlainText: boolean;
    CanWritePlainText: boolean;
    PlainText: string | null;

    CanReadMarkdown: boolean;
    CanWriteMarkdown: boolean;
    Markdown: string | null;

    CanReadHtml: boolean;
    CanWriteHtml: boolean;
    Html: string | null;



    OrganisationsWhereOneData : Organisation[];


    OrganisationsWhereManyData : Organisation[];


}