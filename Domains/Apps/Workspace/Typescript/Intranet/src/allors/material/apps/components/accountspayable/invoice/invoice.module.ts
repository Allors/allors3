import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule, MatCardModule, MatDividerModule, MatFormFieldModule, MatIconModule, MatListModule, MatMenuModule, MatRadioModule, MatToolbarModule, MatTooltipModule, MatOptionModule, MatSelectModule, MatInputModule, MatTabsModule, MatDatepickerModule, MatExpansionModule, MatDialogModule, MatCheckboxModule } from '@angular/material';

import { AllorsMaterialAutoCompleteModule } from '../../../../base/components/autocomplete';
import { AllorsMaterialAvatarModule } from  '../../../../base/components/avatar';
import { AllorsMaterialChipsModule } from '../../../../base/components/chips';
import { AllorsMaterialDatepickerModule } from '../../../../base/components/datepicker';
import { AllorsMaterialFileModule } from '../../../../base/components/file';
import { AllorsMaterialFilesModule } from '../../../../base/components/files';
import { AllorsMaterialInputModule } from '../../../../base/components/input';
import { AllorsMaterialLocalisedTextModule } from '../../../../base/components/localisedtext';
import { AllorsMaterialSelectModule } from '../../../../base/components/select';
import { AllorsMaterialSlideToggleModule } from '../../../../base/components/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../base/components/static';
import { AllorsMaterialTextAreaModule } from '../../../../base/components/textarea';

import { PersonInlineModule } from '../../relations/person/person-inline.module';
import { PartyContactMechanismInlineModule } from '../../relations/party/contactmechanism/party-contactmechanism-inline.module';
import { PartyContactMechanismPostalAddressInlineModule } from '../../relations/party/contactmechanism/postaladdress/party-contactmechanism-postaladdress-inline.module';

import { InvoiceComponent } from './invoice.component';
export { InvoiceComponent } from './invoice.component';

@NgModule({
  declarations: [
    InvoiceComponent,
  ],
  exports: [
    InvoiceComponent,
  ],
  imports: [
    AllorsMaterialAutoCompleteModule,
    AllorsMaterialAvatarModule,
    AllorsMaterialChipsModule,
    AllorsMaterialDatepickerModule,
    AllorsMaterialFileModule,
    AllorsMaterialFilesModule,
    AllorsMaterialInputModule,
    AllorsMaterialLocalisedTextModule,
    AllorsMaterialSelectModule,
    AllorsMaterialSlideToggleModule,
    AllorsMaterialStaticModule,
    AllorsMaterialTextAreaModule,
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatRadioModule,
    MatSelectModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatOptionModule,
    PersonInlineModule,
    PartyContactMechanismInlineModule,
    PartyContactMechanismPostalAddressInlineModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class InvoiceModule { }
