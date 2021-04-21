import { IUnit } from '@allors/workspace/system';
import { MetaPopulation } from '@allors/workspace/json';

interface M extends MetaPopulation {
  Binary: IUnit;

  Boolean: IUnit;

  DateTime: IUnit;

  Decimal: IUnit;

  Float: IUnit;

  Integer: IUnit;

  String: IUnit;

  Unique: IUnit;
}

describe('MetaPopulation', () => {
  describe('default constructor', () => {
    const metaPopulation = new MetaPopulation({}) as M;

    it('should have Binary unit', () => {
      expect(metaPopulation.Binary).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.Boolean).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.DateTime).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.Decimal).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.Float).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.Integer).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.String).not.toBeNull();
    });

    it('should have Binary unit', () => {
      expect(metaPopulation.Unique).not.toBeNull();
    });
  });
});
