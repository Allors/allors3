import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CdkTableModule } from '@angular/cdk/table';
import { MatButtonModule, MatCardModule, MatDividerModule, MatFormFieldModule, MatIconModule, MatListModule, MatMenuModule, MatRadioModule, MatToolbarModule, MatTooltipModule, MatOptionModule, MatSelectModule, MatInputModule, MatTableModule, MatSortModule, MatPaginatorModule } from '@angular/material';
import { RouterModule } from '@angular/router';

import { AllorsMaterialAutoCompleteModule } from '../../../../base/components/autocomplete';
import { AllorsMaterialAvatarModule } from  '../../../../base/components/avatar';
import { AllorsMaterialChipsModule } from '../../../../base/components/chips';
import { AllorsMaterialDatepickerModule } from '../../../../base/components/datepicker';
import { AllorsMaterialFileModule } from '../../../../base/components/file';
import { AllorsMaterialFilesModule } from '../../../../base/components/files';
import { AllorsMaterialInputModule } from '../../../../base/components/input';
import { AllorsMaterialLocalisedTextModule } from '../../../../base/components/localisedtext';
import { AllorsMaterialSelectModule } from '../../../../base/components/select';
import { AllorsMaterialSideNavToggleModule } from '../../../../base/components/sidenavtoggle';
import { AllorsMaterialSlideToggleModule } from '../../../../base/components/slidetoggle';
import { AllorsMaterialStaticModule } from '../../../../base/components/static';
import { AllorsMaterialTextAreaModule } from '../../../../base/components/textarea';

import { OrganisationsOverviewComponent } from './organisations-overview.component';
export { OrganisationsOverviewComponent } from './organisations-overview.component';

@NgModule({
  declarations: [
    OrganisationsOverviewComponent,
  ],
  exports: [
    OrganisationsOverviewComponent,
  ],
  imports: [
    AllorsMaterialAvatarModule,
    AllorsMaterialFileModule,
    AllorsMaterialInputModule,
    AllorsMaterialSelectModule,
    AllorsMaterialSideNavToggleModule,
    AllorsMaterialSlideToggleModule,
    AllorsMaterialStaticModule,
    AllorsMaterialTextAreaModule,
    CommonModule,
    CdkTableModule,
    FlexLayoutModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,  
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatPaginatorModule,
    MatRadioModule,
    MatSelectModule,
    MatSortModule,
    MatTableModule,
    MatToolbarModule,
    MatTooltipModule,
    MatOptionModule,
    ReactiveFormsModule,
    RouterModule,
  ],
})
export class OrganisationsOverviewModule {}
