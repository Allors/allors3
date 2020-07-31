import { AssociationType } from './AssociationType';
import { MetaObject } from './MetaObject';
import { MetaPopulation } from './MetaPopulation';
import { MethodType } from './MethodType';
import { RoleType } from './RoleType';

// TODO: Reverse dependency
import { ids } from '@allors/meta/generated/ids.g';

export enum Kind {
  unit,
  class,
  interface,
}

export class ObjectType implements MetaObject {

  interfaces: ObjectType[];
  subtypes: ObjectType[];
  classes: ObjectType[];

  exclusiveRoleTypes: RoleType[];
  exclusiveAssociationTypes: AssociationType[];
  exclusiveMethodTypes: MethodType[];

  roleTypes: RoleType[];
  associationTypes: AssociationType[];
  methodTypes: MethodType[];

  roleTypeByName: Map<string, RoleType>;
  associationTypeByName: Map<string, AssociationType>;
  methodTypeByName: Map<string, MethodType>;

  constructor(
    public metaPopulation: MetaPopulation,
    public id: string,
    public name: string,
    public plural: string,
    public kind: Kind
  ) {
    this.interfaces = [];
    this.subtypes = [];
    this.classes = [];

    this.exclusiveRoleTypes = [];
    this.exclusiveAssociationTypes = [];
    this.exclusiveMethodTypes = [];

    this.roleTypes = [];
    this.associationTypes = [];
    this.methodTypes = [];

    this.roleTypeByName = new Map();
    this.associationTypeByName = new Map();
    this.methodTypeByName = new Map();
  }

  get isUnit(): boolean {
    return this.kind === Kind.unit;
  }

  get isBinary(): boolean {
    return this.id === ids.Binary;
  }

  get isBoolean(): boolean {
    return this.id === ids.Boolean;
  }

  get isDateTime(): boolean {
    return this.id === ids.DateTime;
  }

  get isDecimal(): boolean {
    return this.id === ids.Decimal;
  }

  get isFloat(): boolean {
    return this.id === ids.Float;
  }

  get isInteger(): boolean {
    return this.id === ids.Integer;
  }

  get isString(): boolean {
    return this.id === ids.String;
  }

  get isUnique(): boolean {
    return this.id === ids.Unique;
  }

  get isComposite(): boolean {
    return this.kind !== Kind.unit;
  }

  get isInterface(): boolean {
    return this.kind === Kind.interface;
  }

  get isClass(): boolean {
    return this.kind === Kind.class;
  }
}
