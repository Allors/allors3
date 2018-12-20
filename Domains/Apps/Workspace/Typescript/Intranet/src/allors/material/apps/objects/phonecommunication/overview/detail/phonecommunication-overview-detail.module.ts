import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule, MatCardModule, MatDividerModule, MatFormFieldModule, MatIconModule, MatListModule, MatMenuModule, MatRadioModule, MatToolbarModule, MatTooltipModule, MatOptionModule, MatSelectModule, MatInputModule } from '@angular/material';

import { AllorsMaterialDatepickerModule } from '../../../../../base/components/role/datepicker';
import { AllorsMaterialDatetimepickerModule } from '../../../../../base/components/role/datetimepicker';
import { AllorsMaterialChipsModule } from '../../../../../base/components/role/chips';
import { AllorsMaterialFileModule } from '../../../../../base/components/role/file';
import { AllorsMaterialHeaderModule } from '../../../../../base/components/header';
import { AllorsMaterialInputModule } from '../../../../../base/components/role/input';
import { AllorsMaterialSelectModule } from '../../../../../base/components/role/select';
import { AllorsMaterialSideNavToggleModule } from '../../../../../base/components/sidenavtoggle';
import { AllorsMaterialSlideToggleModule } from '../../../../../base/components/role/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../../base/components/role/static';
import { AllorsMaterialTextAreaModule } from '../../../../../base/components/role/textarea';
import { AllorsMaterialFooterModule } from '../../../../../base/components/footer';

import { PersonInlineModule } from '../../../person/inline/person-inline.module';
import { TelecommunicationsNumberInlineModule } from '../../../telecommunicationsnumber/inline/telecommunicationsnumber-inline.module';

import { PhoneCommunicationOverviewDetailComponent } from './phonecommunication-overview-detail.component';
export { PhoneCommunicationOverviewDetailComponent } from './phonecommunication-overview-detail.component';

@NgModule({
  declarations: [
    PhoneCommunicationOverviewDetailComponent,
  ],
  exports: [
    PhoneCommunicationOverviewDetailComponent,
  ],
  imports: [
    PersonInlineModule,
    TelecommunicationsNumberInlineModule,

    AllorsMaterialChipsModule,
    AllorsMaterialDatepickerModule,
    AllorsMaterialDatetimepickerModule,
    AllorsMaterialFileModule,
    AllorsMaterialHeaderModule,
    AllorsMaterialFooterModule,
    AllorsMaterialInputModule,
    AllorsMaterialSelectModule,
    AllorsMaterialSideNavToggleModule,
    AllorsMaterialSlideToggleModule,
    AllorsMaterialStaticModule,
    AllorsMaterialTextAreaModule,
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatRadioModule,
    MatSelectModule,
    MatToolbarModule,
    MatTooltipModule,
    MatOptionModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class PhoneCommunicationOverviewDetailModule { }