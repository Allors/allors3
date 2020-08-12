import { Injectable } from '@angular/core';

import { RoleType } from '@allors/meta/system';
import { RefreshService } from '@allors/angular/core';
import { ObjectService } from '@allors/angular/material/core';

import { EditAction } from './EditAction';

@Injectable({
  providedIn: 'root',
})
export class EditService {

  constructor(
    private objectService: ObjectService,
    private refreshService: RefreshService,
    ) {}

  edit(roleType?: RoleType) {
    return new EditAction(this.objectService, this.refreshService, roleType);
  }

}
