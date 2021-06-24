import { InvokeRequest, InvokeResponse, PullRequest, PullResponse, PushRequest, PushResponse, SecurityRequest, SecurityResponse, SyncRequest, SyncResponse } from '@allors/protocol/json/system';
import { Configuration, Database as SystemDatabase, equals, IdGenerator, MapMap, ServicesBuilder } from '@allors/workspace/adapters/system';
import { IWorkspace, Operations } from '@allors/workspace/domain/system';
import { Class, MethodType, OperandType, RelationType } from '@allors/workspace/meta/system';
import { Client } from './Client';
import { DatabaseRecord } from './DatabaseRecord';
import { AccessControl } from './security/AccessControl';
import { ResponseContext } from './Security/ResponseContext';
import { Workspace } from './Workspace';

export class Database extends SystemDatabase {
  recordsById: Map<number, DatabaseRecord>;

  accessControlById: Map<number, AccessControl>;
  permissions: Set<number>;

  readPermissionByOperandTypeByClass: MapMap<Class, OperandType, number>;
  writePermissionByOperandTypeByClass: MapMap<Class, OperandType, number>;
  executePermissionByOperandTypeByClass: MapMap<Class, OperandType, number>;

  constructor(configuration: Configuration, servicesBuilder: ServicesBuilder, idGenerator: IdGenerator, public client: Client) {
    super(configuration, servicesBuilder, idGenerator);

    this.recordsById = new Map();

    this.accessControlById = new Map();
    this.permissions = new Set();

    this.readPermissionByOperandTypeByClass = new MapMap();
    this.writePermissionByOperandTypeByClass = new MapMap();
    this.executePermissionByOperandTypeByClass = new MapMap();
  }

  createWorkspace(): IWorkspace {
    return new Workspace(this, this.servicesBuilder());
  }

  onPushResponse(cls: Class, id: number): DatabaseRecord {
    const record = new DatabaseRecord(this, cls, id, 0);
    this.recordsById.set(id, record);
    return record;
  }

  onSyncResponse(syncResponse: SyncResponse): SecurityRequest | null {
    const ctx = new ResponseContext(this);
    for (const syncResponseObject of syncResponse.o) {
      const databaseObjects = DatabaseRecord.fromResponse(this, ctx, syncResponseObject);
      this.recordsById.set(databaseObjects.id, databaseObjects);
    }

    if (ctx.missingAccessControlIds.size > 0 || ctx.missingPermissionIds.size > 0) {
      return {
        a: Array.from(ctx.missingAccessControlIds),
        p: Array.from(ctx.missingPermissionIds),
      };
    }

    return null;
  }

  securityResponse(securityResponse: SecurityResponse): SecurityRequest | undefined {
    if (securityResponse.p != null) {
      for (const syncResponsePermission of securityResponse.p) {
        const id = syncResponsePermission[0];
        const cls = this.configuration.metaPopulation.metaObjectByTag.get(syncResponsePermission[1]) as Class;
        const metaObject = this.configuration.metaPopulation.metaObjectByTag.get(syncResponsePermission[2]);
        const operandType: OperandType = (metaObject as RelationType)?.roleType ?? (metaObject as MethodType);
        const operation = syncResponsePermission[3];

        this.permissions.add(id);

        switch (operation) {
          case Operations.Read:
            this.readPermissionByOperandTypeByClass.set(cls, operandType, id);
            break;
          case Operations.Write:
            this.writePermissionByOperandTypeByClass.set(cls, operandType, id);
            break;
          case Operations.Execute:
            this.executePermissionByOperandTypeByClass.set(cls, operandType, id);
            break;
        }
      }
    }

    let missingPermissionIds: Set<number> | undefined = undefined;
    if (securityResponse.a != null) {
      for (const syncResponseAccessControl of securityResponse.a) {
        const id = syncResponseAccessControl.i;
        const version = syncResponseAccessControl.v;
        const permissionsIds = syncResponseAccessControl.p?.map((v) => {
          if (this.permissions.has(v)) {
            return v;
          }

          (missingPermissionIds ??= new Set()).add(v);

          return v;
        });

        const permissionIdSet = permissionsIds != null ? new Set(permissionsIds) : new Set<number>();

        this.accessControlById.set(id, new AccessControl(version, permissionIdSet));
      }
    }

    if (missingPermissionIds) {
      return {
        p: Array.from(missingPermissionIds),
      };
    }

    return undefined;
  }

  onPullResonse(response: PullResponse): SyncRequest {
    return {
      o: response.p
        .filter((v) => {
          const record = this.recordsById.get(v.i);

          if (!record) {
            return true;
          }

          if (record.version !== v.v) {
            return true;
          }

          if (!equals(record.accessControlIds, v.a)) {
            return true;
          }

          if (!equals(record.deniedPermissionIds, v.d)) {
            return true;
          }

          return false;
        })
        .map((v) => v.i),
    };
  }

  getPermission(cls: Class, operandType: OperandType, operation: Operations): number | undefined {
    switch (operation) {
      case Operations.Read:
        return this.readPermissionByOperandTypeByClass.get(cls, operandType);
      case Operations.Write:
        return this.writePermissionByOperandTypeByClass.get(cls, operandType);
      default:
        return this.executePermissionByOperandTypeByClass.get(cls, operandType);
    }
  }

  // pull(name: string, values?: Map<string, object>, objects: Map<string, IObject>, collections: Map<string, IObject[]>): Observable<PullResponse> {
  //   // const pullArgs: PullArgs = {
  //   //     v: values?.ToDictionary(v => v.Key, v => v.Value),
  //   //     o: objects?.ToDictionary(v => v.Key, v => v.Value.Id),
  //   //     c: collections?.ToDictionary(v => v.Key, v => v.Value.Select(v => v.Id).ToArray()),
  //   // };
  //   // var uri = new Uri(name + "/pull", UriKind.Relative);
  //   // var response = await this.PostAsJsonAsync(uri, pullArgs);
  //   // _ = response.EnsureSuccessStatusCode();
  //   // return await this.ReadAsAsync<PullResponse>(response);

  //   // TODO:
  //   return undefined;
  // }

  getRecord(identity: number): DatabaseRecord | undefined {
    return this.recordsById.get(identity);
  }

  pull(pullRequest: PullRequest): Promise<PullResponse> {
    return this.client.pull(pullRequest);
  }

  sync(syncRequest: SyncRequest): Promise<SyncResponse> {
    return this.client.sync(syncRequest);
  }

  push(pushRequest: PushRequest): Promise<PushResponse> {
    return this.client.push(pushRequest);
  }

  invoke(invokeRequest: InvokeRequest): Promise<InvokeResponse> {
    return this.client.invoke(invokeRequest);
  }

  security(securityRequest: SecurityRequest): Promise<SecurityResponse> {
    return this.client.security(securityRequest);
  }
}
