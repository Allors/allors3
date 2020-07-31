import { serialize } from '../workspace/SessionObject';
import { ParameterizablePredicateArgs, ParameterizablePredicate } from './ParameterizablePredicate';
import { RoleType } from '../meta/RoleType';
import { UnitTypes } from '../workspace/Types';
import { ObjectType } from '../meta/ObjectType';

export interface GreaterThanArgs extends ParameterizablePredicateArgs, Pick<GreaterThan, 'roleType' | 'value'> {}

export class GreaterThan extends ParameterizablePredicate {
  public roleType: RoleType;
  public value?: UnitTypes;

  constructor(roleType: RoleType);
  constructor(args: GreaterThanArgs);
  constructor(args: GreaterThanArgs | RoleType) {
    super();

    if (args instanceof RoleType) {
      this.roleType = args;
    } else {
      Object.assign(this, args);
      this.roleType = args.roleType;
    }
  }

  get objectType(): ObjectType {
    return this.roleType.objectType;
  }

  public toJSON(): any {
    return {
      kind: 'GreaterThan',
      dependencies: this.dependencies,
      roleType: this.roleType.id,
      parameter: this.parameter,
      value: serialize(this.value),
    };
  }
}
