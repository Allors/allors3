import { Injectable } from '@angular/core';
import { Route, Router, ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { NavigationItem } from './NavigationItem';
import { ISessionObject, ObjectType, MetaObject, ObjectTypeRef } from 'src/allors/framework';
import { WorkspaceService } from '../framework';

@Injectable()
export class NavigationService {
  public navigationItems: NavigationItem[];

  constructor(
    workspaceService: WorkspaceService,
    private router: Router,
    private location: Location
  ) {
    this.navigationItems = [];
    this.router.config.map((route: Route) => new NavigationItem(workspaceService.metaPopulation, this.navigationItems, route));
  }

  list(objectTypeOrMetaObjectType: ObjectType | ObjectTypeRef) {
    const objectTypeId = (objectTypeOrMetaObjectType instanceof ObjectType) ? objectTypeOrMetaObjectType.id : objectTypeOrMetaObjectType.objectType.id;
    const navigationItem = this.navigationItems.find((v) => v.id === objectTypeId && v.action === 'list');
    const url = navigationItem.link;
    this.router.navigate([url]);
  }

  overview(sessionObject: ISessionObject) {
    const objectTypeId = sessionObject.objectType.id;
    const navigationItem = this.navigationItems.find((v) => v.id === objectTypeId && v.action === 'overview') ||
      this.navigationItems.find((v) => v.ids && v.ids.indexOf(objectTypeId) > -1 && v.action === 'overview');
    const url = navigationItem.link.replace(`:id`, sessionObject.id);
    this.router.navigate([url]);
  }

  add(objectTypeOrMetaObjectType: ObjectType | ObjectTypeRef, ...params: ISessionObject[]) {
    const objectTypeId = (objectTypeOrMetaObjectType instanceof ObjectType) ? objectTypeOrMetaObjectType.id : objectTypeOrMetaObjectType.objectType.id;
    const navigationItem = this.navigationItems.find((v) => v.id === objectTypeId && v.action === 'add');
    const url = navigationItem.link;
    const queryParams = params.reduce((acc, v) => { acc[v.objectType.name] = v.id; return acc; }, {});
    this.router.navigate([url], { queryParams });
  }

  edit(sessionObject: ISessionObject, ...params: ISessionObject[]) {
    const objectTypeId = sessionObject.objectType.id;
    const navigationItem = this.navigationItems.find((v) => v.id === objectTypeId && v.action === 'edit');
    const url = navigationItem.link.replace(`:id`, sessionObject.id);
    const queryParams = params.reduce((acc, v) => { acc[v.objectType.name] = v.id; return acc; }, {});
    this.router.navigate([url], { queryParams });
  }

  back() {
    this.location.back();
  }
}
