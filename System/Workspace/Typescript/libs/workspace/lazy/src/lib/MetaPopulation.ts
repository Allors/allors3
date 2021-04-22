import { IComposite, IInterface, IMetaObject, IObjectType, MetaData } from '@allors/workspace/system';
import { IMetaPopulationInternals } from './Internals/IMetaPopulationInternals';
import { Class } from './Class';
import { Interface } from './Interface';
import { Unit } from './Unit';
import { RelationType } from './RelationType';
import { MethodType } from './MethodType';
import { Composite } from './Composite';

export class MetaPopulation implements IMetaPopulationInternals {
  constructor(data: MetaData) {
    this.units = ['Binary', 'Boolean', 'DateTime', 'Decimal', 'Float', 'Integer', 'String', 'Unique'].map((name, i) => new Unit(this, i + 1, name));
    this.interfaces = data.i?.map((v) => new Interface(this, v)) ?? [];
    this.classes = data.c?.map((v) => new Class(this, v)) ?? [];
    this.composites = (this.classes as Composite[]).concat(this.interfaces);
    this.compositeByName = this.composites.reduce((dic, v) => (dic[v.singularName] = v), {});
    data.h?.forEach(([sup, subs]) => {
      const supertype = this.metaObjectByTag[sup] as IInterface;
      const directSubtypes = subs?.map((w) => this.metaObjectByTag[w] as IComposite) ?? [];
      supertype.directSubtypes = directSubtypes;
      directSubtypes.forEach((v) => {
        if (v.directSupertypes) {
          v.directSupertypes.push(supertype);
        } else {
          v.directSupertypes = [supertype];
        }
      });
    });
    this.composites.forEach(v=>v.init());
  }

  readonly metaObjectByTag: IMetaObject[] = [];
  readonly units: Unit[];
  readonly interfaces: Interface[];
  readonly classes: Class[];
  readonly composites: Composite[];
  readonly compositeByName: { [name: string]: IComposite } = {};
  readonly relationTypes: RelationType[];
  readonly methodTypes: MethodType[];

  onMetaObject(objectType: IObjectType): void {
    this.metaObjectByTag[objectType.tag] = objectType;
    this[objectType.singularName] = objectType;
  }
}
