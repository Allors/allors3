import { ObjectType, AssociationType, RoleType, MethodType, OperandType } from '@allors/meta/system';
import { Operations, PushRequestObject, PushRequestNewObject } from '@allors/protocol/system';

import { Session } from './Session';
import { DatabaseObject } from './DatabaseObject';

export interface Object {
  readonly id: string;
  readonly objectType: ObjectType;
}

export interface SessionObject extends Object {
  readonly id: string;
  readonly objectType: ObjectType;
  readonly newId?: string;
  readonly version: string | undefined;

  readonly isNew: boolean;

  readonly session: Session;
  readonly workspaceObject?: DatabaseObject;

  readonly hasChanges: boolean;

  canRead(roleType: RoleType): boolean | undefined;
  canWrite(roleTyp: RoleType): boolean | undefined;
  canExecute(methodType: MethodType): boolean | undefined;
  isPermited(operandType: OperandType, operation: Operations): boolean | undefined;

  get(roleType: RoleType): any;
  set(roleType: RoleType, value: any): void;
  add(roleType: RoleType, value: any): void;
  remove(roleType: RoleType, value: any): void;

  getAssociation(associationType: AssociationType): any;

  save(): PushRequestObject | undefined;
  saveNew(): PushRequestNewObject;
  reset(): void;
}
