import js from '@eslint/js';
import eslintConfigPrettier from 'eslint-config-prettier';
import reactHooks from 'eslint-plugin-react-hooks';
import reactRefresh from 'eslint-plugin-react-refresh';
import globals from 'globals';
import tseslint from 'typescript-eslint';
// import filenamesPlugin from 'eslint-plugin-filenames';

export default tseslint.config(
  { ignores: ['dist'] },
  {
    extends: [js.configs.recommended, ...tseslint.configs.recommended],
    files: ['**/*.{ts,tsx}'],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    plugins: {
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
      // filenames: filenamesPlugin,
    },
    rules: {
      ...reactHooks.configs.recommended.rules,
      'react-refresh/only-export-components': ['warn', { allowConstantExport: true }],
      // Prefer function declarations for components and regular functions
      'func-style': ['error', 'declaration', { allowArrowFunctions: true }],
      // Enforce kebab-case filenames
      // 'filenames/match-regex': ['error', '^[a-z0-9.-]+$'],
    },
  },
  eslintConfigPrettier
);
