module.exports = {
    preset: 'jest-preset-angular',
    setupFilesAfterEnv: ['<rootDir>/src/test.ts'],
    globals: {
      'ts-jest': {
        tsconfig: 'tsconfig.spec.json',
      },
    },
    transform: {
      '^.+\\.(ts|js|html)$': 'ts-jest',
    },
    testEnvironment: 'jest-environment-jsdom',
  };
  