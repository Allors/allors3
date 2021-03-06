import { ObjectType } from '@allors/workspace/meta/system';
import { Predicate } from './Predicate';
import { Sort } from './Sort';

export interface Filter {
  kind: 'Filter';
  
  objectType: ObjectType;

  predicate?: Predicate;

  sorting?: Sort[];
}
