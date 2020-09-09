import { Meta } from '@allors/meta/generated';
import { assert } from '@allors/meta/system';
import { UnifiedGood } from '@allors/domain/generated';
import { Database } from '@allors/workspace/system';

export function extendUnifiedGood(workspace: Database) {
  const m = workspace.metaPopulation as Meta;
  const cls = workspace.constructorByObjectType.get(m.UnifiedGood);
  assert(cls);

  Object.defineProperty(cls.prototype, 'categoryNames', {
    configurable: true,
    get(this: UnifiedGood) {
      return this.ProductCategoriesWhereProduct.map((v) => v.displayName).join(', ');
    },
  });
}
