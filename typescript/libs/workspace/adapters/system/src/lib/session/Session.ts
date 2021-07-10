import { IChangeSet, InvokeOptions, IObject, IPullResult, IPushResult, IResult, ISession, ISessionServices, Method, Procedure, Pull } from '@allors/workspace/domain/system';
import { Workspace } from '../workspace/Workspace';
import { SessionOriginState } from './originstate/SessionOriginState';
import { Strategy } from './Strategy';
import { ChangeSetTracker } from './trackers/ChangSetTracker';
import { PushToDatabaseTracker } from './trackers/PushToDatabaseTracker';
import { PushToWorkspaceTracker } from './trackers/PushToWorkspaceTracker';
import { ChangeSet } from './ChangeSet';
import { enumerate } from '../collections/Range';
import { AssociationType, Class, Composite, Origin, RoleType } from '@allors/workspace/meta/system';

export function isNewId(id: number): boolean {
  return id < 0;
}

export abstract class Session implements ISession {
  changeSetTracker: ChangeSetTracker;

  pushToDatabaseTracker: PushToDatabaseTracker;

  pushToWorkspaceTracker: PushToWorkspaceTracker;

  sessionOriginState: SessionOriginState;

  strategyByWorkspaceId: Map<number, Strategy>;

  private strategiesByClass: Map<Class, Set<Strategy>>;

  constructor(public workspace: Workspace, public services: ISessionServices) {
    this.strategyByWorkspaceId = new Map();
    this.strategiesByClass = new Map();
    this.sessionOriginState = new SessionOriginState();
    this.changeSetTracker = new ChangeSetTracker();
    this.pushToDatabaseTracker = new PushToDatabaseTracker();
    this.pushToWorkspaceTracker = new PushToWorkspaceTracker();
  }

  abstract Create(cls: Composite): IObject;

  abstract invoke(method: Method | Method[], options?: InvokeOptions): Promise<IResult>;

  abstract call(procedure: Procedure, ...pulls: Pull[]): Promise<IPullResult>;

  abstract pull(pulls: Pull[]): Promise<IPullResult>;

  abstract push(): Promise<IResult>;

  getOne<T>(id: number): T {
    return (this.getStrategy(id)?.object as unknown) as T;
  }

  getMany<T>(ids: number[]): T[] {
    return (ids.map((v) => this.getStrategy(v)).filter((v) => v != null) as unknown) as T[];
  }

  getAll<T extends IObject>(objectType: Composite): T[] {
    const all: T[] = [];

    for (const cls of objectType.classes) {
      switch (cls.origin) {
        case Origin.Workspace:
          if (this.workspace.workspaceIdsByWorkspaceClass.has(cls)) {
            const ids = this.workspace.workspaceIdsByWorkspaceClass.get(cls);
            for (const id of ids) {
              if (this.strategyByWorkspaceId.has(id)) {
                const strategy = this.strategyByWorkspaceId.get(id);
                all.push(strategy.object as T);
              } else {
                const strategy = this.instantiateWorkspaceStrategy(id);
                all.push(strategy.object as T);
              }
            }
          }

          break;
      }
    }

    return all;
  }

  checkpoint(): IChangeSet {
    const changeSet = new ChangeSet(this, this.changeSetTracker.Created, this.changeSetTracker.Instantiated);
    if (this.changeSetTracker.DatabaseOriginStates != null) {
      for (const databaseOriginState of this.changeSetTracker.DatabaseOriginStates) {
        databaseOriginState.Checkpoint(changeSet);
      }
    }

    if (this.changeSetTracker.WorkspaceOriginStates != null) {
      for (const workspaceOriginState of this.changeSetTracker.WorkspaceOriginStates) {
        workspaceOriginState.Checkpoint(changeSet);
      }
    }

    this.sessionOriginState.Checkpoint(changeSet);
    this.changeSetTracker.Created = null;
    this.changeSetTracker.Instantiated = null;
    this.changeSetTracker.DatabaseOriginStates = null;
    this.changeSetTracker.WorkspaceOriginStates = null;

    return changeSet;
  }

  public getStrategy(id: number): Strategy {
    if (id == 0) {
      return;
    }

    if (this.strategyByWorkspaceId.has(id)) {
      return this.strategyByWorkspaceId.get(id);
    }

    if (isNewId(id)) {
      this.instantiateWorkspaceStrategy(id);
    }

    return null;
  }

  public getRole(association: Strategy, roleType: RoleType): any {
    const role = this.sessionOriginState.Get(association.id, roleType);
    if (roleType.objectType.isUnit) {
      return role;
    }

    if (roleType.isOne) {
      return this.getOne<IObject>(role);
    }

    if (role !== null) {
      const roles = [];
      for (const v of enumerate(role)) {
        roles.push(this.getOne<IObject>(v));
      }
    } else {
      return [];
    }
  }

  public getCompositeAssociation<T extends IObject>(role: number, associationType: AssociationType): T {
    const roleType = associationType.roleType;
    for (const association of this.getForAssociation(associationType.objectType as Composite)) {
      if (association.canRead(roleType)) {
        if (association.isCompositeAssociationForRole(roleType, role)) {
          return association.object as T;
        }
      }
    }
  }

  public getCompositesAssociation<T extends IObject>(role: number, associationType: AssociationType): T[] {
    const roleType = associationType.roleType;

    const associations: T[] = [];

    for (const association of this.getForAssociation(associationType.objectType as Composite)) {
      if (!association.canRead(roleType)) {
        // TODO: Warning!!! continue If
      }

      if (association.isCompositesAssociationForRole(roleType, role)) {
        associations.push(association.object as T);
      }
    }

    return associations;
  }

  protected addStrategy(strategy: Strategy) {
    this.strategyByWorkspaceId.set(strategy.id, strategy);

    let strategies = this.strategiesByClass.get(strategy.cls);
    if (strategies == null) {
      strategies = new Set();
      this.strategiesByClass.set(strategy.cls, strategies);
    }

    strategies.add(strategy);
  }

  protected removeStrategy(strategy: Strategy) {
    this.strategyByWorkspaceId.delete(strategy.id);

    const strategies = this.strategiesByClass.get(strategy.cls);
    if (strategies != null) {
      strategies.delete(strategy);
    }
  }

  protected pushToWorkspace(result: IPushResult): IPushResult {
    if (this.pushToWorkspaceTracker.Created != null) {
      for (const strategy of this.pushToWorkspaceTracker.Created) {
        strategy.WorkspaceOriginState.Push();
      }
    }

    if (this.pushToWorkspaceTracker.Changed != null) {
      for (const state of this.pushToWorkspaceTracker.Changed) {
        if (this.pushToWorkspaceTracker.Created?.has(state.strategy) == true) {
          continue;
        }

        state.Push();
      }
    }

    this.pushToWorkspaceTracker.Created = null;
    this.pushToWorkspaceTracker.Changed = null;
    return result;
  }

  protected onDatabasePushResponseNew(workspaceId: number, databaseId: number) {
    const strategy = this.strategyByWorkspaceId[workspaceId];
    this.pushToDatabaseTracker.Created.delete(strategy);
    this.removeStrategy(strategy);
    strategy.OnDatabasePushNewId(databaseId);
    this.addStrategy(strategy);
    this.onDatabasePushResponse(strategy);
  }

  protected onDatabasePushResponse(strategy: Strategy) {
    const databaseRecord = this.workspace.database.onPushResponse(strategy.cls, strategy.id);
    strategy.onDatabasePushResponse(databaseRecord);
  }

  private getForAssociation(objectType: Composite): Strategy[] {
    const classes = objectType.classes;

    const strategies: Strategy[] = [];

    for (const [_, strategy] of this.strategyByWorkspaceId) {
      if (classes.has(strategy.cls)) {
        strategies.push(strategy);
      }
    }

    return strategies;
  }

  abstract instantiateDatabaseStrategy(id: number): Strategy;

  abstract instantiateWorkspaceStrategy(id: number): Strategy;
}
