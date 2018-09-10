import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule, MatInputModule } from '@angular/material';

import { AllorsMaterialAutocompleteComponent } from './autocomplete.component';
export { AllorsMaterialAutocompleteComponent } from './autocomplete.component';

@NgModule({
  declarations: [
    AllorsMaterialAutocompleteComponent,
  ],
  exports: [
    AllorsMaterialAutocompleteComponent,
  ],
  imports: [
    CommonModule,

    FormsModule,
    MatAutocompleteModule,
    MatInputModule,
    ReactiveFormsModule,
  ],
})
export class AllorsMaterialAutoCompleteModule {
}
