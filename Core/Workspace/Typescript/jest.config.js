module.exports = {
  projects: [
    '<rootDir>/apps/angular/app',
    '<rootDir>/apps/angular/material/app',
    '<rootDir>/apps/gatsby',
    '<rootDir>/libs/client/fetch',
    '<rootDir>/libs/client/tests',
    '<rootDir>/libs/data/core',
    '<rootDir>/libs/domain/core',
    '<rootDir>/libs/domain/custom',
    '<rootDir>/libs/domain/generated',
    '<rootDir>/libs/meta/core',
    '<rootDir>/libs/meta/generated',
    '<rootDir>/libs/meta/tests',
    '<rootDir>/libs/workspace/core',
    '<rootDir>/libs/workspace/tests',
    '<rootDir>/libs/protocol/json/system',
    '<rootDir>/libs/workspace/system',
    '<rootDir>/libs/adapters/memory/system',
    '<rootDir>/libs/meta/lazy/system',
  ],
  setupFilesAfterEnv: ['jest-extended', 'jest-chain'],
};
