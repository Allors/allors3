import { IRange } from '@allors/workspace/adapters/system';

export class AccessControl {
  constructor(public version: number, public permissionIds: IRange) {}
}
