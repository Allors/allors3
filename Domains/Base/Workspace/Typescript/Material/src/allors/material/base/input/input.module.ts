import { NgModule } from "@angular/core";

import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";

import { MatAutocompleteModule, MatChipsModule, MatIconModule, MatInputModule } from "@angular/material";

import { InputComponent } from "./input.component";
export { InputComponent } from "./input.component";

@NgModule({
  declarations: [
    InputComponent,
  ],
  exports: [
    InputComponent,
  ],
  imports: [
    FormsModule,
    CommonModule,
    MatInputModule,
    MatIconModule,
    MatChipsModule,
    MatAutocompleteModule,
  ],
})
export class InputModule {
}
