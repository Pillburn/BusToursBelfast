// stores/StoreContext.ts
import { createContext, useContext, type ReactNode, createElement } from 'react';
import { RootStore } from './index.ts';
import { rootStore } from './index.ts';

export const StoreContext = createContext<RootStore | null>(null);

export function StoreProvider({ children }: { children: ReactNode }) {
  return createElement(
    StoreContext.Provider,
    { value: rootStore },
    children
  );
}

export const useStore = () => {
  const context = useContext(StoreContext);
  if (context === null) {
    throw new Error('useStore must be used within a StoreProvider');
  }
  return context;
};