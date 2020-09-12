import { SessionObject, ParameterTypes } from '@allors/workspace/system';

import { IExtent } from './IExtent';
import { Result } from './Result';
import { Fetch } from './Fetch';
import { Predicate } from './Predicate';
import { Sort } from './Sort';
import { Tree } from './Tree';

export interface FlatPull {
  extentRef?: string;

  extent?: IExtent;

  predicate?: Predicate;

  sort?: Sort | Sort[];

  object?: SessionObject | string;

  results?: Result[];

  fetchRef?: string;

  fetch?: Fetch | any;

  include?: Tree | any;

  parameters?: { [id: string]: ParameterTypes };

  name?: string;

  skip?: number;

  take?: number;
}
