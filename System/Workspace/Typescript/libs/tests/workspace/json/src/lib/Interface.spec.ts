import { IInterface, IMetaPopulation} from '@allors/workspace/system';
import { MetaData, MetaPopulation } from '@allors/workspace/json';

type Named = IInterface;

interface M extends IMetaPopulation {
  Named: Named;
}

describe('MetaPopulation', () => {
  describe('constructor with minimal interface metadata', () => {
    const data: MetaData = {
      i: [{ t: 9, s: 'Named' }],
    };

    const metaPopulation = new MetaPopulation(data) as M;

    it('should have the interface', () => {
      expect(metaPopulation.Named).not.toBeNull();
    });

    describe('with interface', () => {
      const named = metaPopulation.Named;
      it('should have properties ', () => {
        expect(named.tag).toBe(9);
        expect(named.singularName).toBe('Named');
      });
    });
  });
});
