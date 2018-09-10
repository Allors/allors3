import { NgModule } from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatInputModule } from '@angular/material';

import { AllorsMaterialStaticComponent } from './static.component';

export { AllorsMaterialStaticComponent } from './static.component';

@NgModule({
  declarations: [
    AllorsMaterialStaticComponent,
  ],
  exports: [
    AllorsMaterialStaticComponent,
  ],
  imports: [

    FormsModule,
    CommonModule,
    MatInputModule,
  ],
})
export class AllorsMaterialStaticModule {
}
