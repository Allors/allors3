module.exports = {
  displayName: 'workspace-domain-json-ajax-core',
  preset: '../../../../../../jest.preset.js',
  globals: {
    'ts-jest': {
      tsConfig: '<rootDir>/tsconfig.spec.json',
    },
  },
  transform: {
    '^.+\\.[tj]sx?$': 'ts-jest',
  },
  moduleFileExtensions: ['ts', 'tsx', 'js', 'jsx'],
  coverageDirectory: '../../../../../../coverage/libs/workspace/domain/json/ajax/core',
};
